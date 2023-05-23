namespace OpenMedStack.Autofac.NEventstore.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Domain;
using NEventStore;
using NEventStore.Persistence;
using OpenMedStack.Events;

public class SagaEventStoreRepository : ISagaRepository
{
    private readonly IProvideTenant _tenantId;
    private readonly IStoreEvents _eventStore;
    private readonly IConstructSagas _factory;
    private readonly ILogger<SagaEventStoreRepository> _logger;

    public SagaEventStoreRepository(IProvideTenant tenantId, IStoreEvents eventStore, IConstructSagas factory, ILogger<SagaEventStoreRepository> logger)
    {
        _tenantId = tenantId;
        _eventStore = eventStore;
        _factory = factory;
        _logger = logger;
    }

    public async Task<TSaga> GetById<TSaga>(string sagaId) where TSaga : class, ISaga
    {
        var eventStream = await OpenStream(_tenantId.GetTenantName(), sagaId).ConfigureAwait(false);
        return BuildSaga<TSaga>(sagaId, eventStream);
    }

    public async Task Save(ISaga saga, Action<IDictionary<string, object>>? updateHeaders, CancellationToken cancellationToken)
    {
        if (saga == null)
        {
            throw new ArgumentNullException(nameof(saga), "Saga cannot be null");
        }
        var commitId = Guid.NewGuid();
        var headers = PrepareHeaders(saga, updateHeaders);
        var eventStream = await PrepareStream(_tenantId.GetTenantName(), saga, headers).ConfigureAwait(false);
        await Persist(eventStream, commitId, cancellationToken).ConfigureAwait(false);
        saga.ClearUncommittedEvents();
        saga.ClearUndispatchedMessages();
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private async Task<IEventStream> OpenStream(string bucketId, string sagaId, int maxVersion = int.MaxValue, CancellationToken cancellationToken = default)
    {
        IEventStream eventStream;
        try
        {
            eventStream = await _eventStore.OpenStream(bucketId, sagaId, 0, maxVersion, cancellationToken).ConfigureAwait(false);
        }
        catch (StreamNotFoundException)
        {
            eventStream = await _eventStore.CreateStream(bucketId, sagaId).ConfigureAwait(false);
        }
        return eventStream;
    }

    private TSaga BuildSaga<TSaga>(string sagaId, IEventStream stream) where TSaga : class, ISaga
    {
        var saga = (TSaga)_factory.Build(typeof(TSaga), sagaId);
        foreach (var message in stream.CommittedEvents.Select(x => x.Body).OfType<DomainEvent>().ToArray())
        {
            saga.Transition(message);
        }
        saga.ClearUncommittedEvents();
        saga.ClearUndispatchedMessages();
        return saga;
    }

    private static Dictionary<string, object> PrepareHeaders(ISaga saga, Action<IDictionary<string, object>>? updateHeaders)
    {
        var dictionary = new Dictionary<string, object> { ["SagaType"] = saga.GetType().FullName! };
        updateHeaders?.Invoke(dictionary);
        var num = 0;
        foreach (var undispatchedMessage in saga.GetUndispatchedMessages())
        {
            dictionary["UndispatchedMessage." + num++] = undispatchedMessage;
        }
        return dictionary;
    }

    private async Task<IEventStream> PrepareStream(string bucketId, ISaga saga, Dictionary<string, object> headers)
    {
        var stream = await OpenStream(bucketId, saga.Id, saga.Version).ConfigureAwait(false);
        //.CreateStream(bucketId, saga.Id).ConfigureAwait(false);

        foreach (var (key, value) in headers)
        {
            stream.UncommittedHeaders[key] = value;
        }

        foreach (var msg in saga.GetUncommittedEvents().Select(x => new EventMessage(x)))
        {
            stream.Add(msg);
        }

        return stream;
    }

    private async Task Persist(IEventStream stream, Guid commitId, CancellationToken cancellationToken)
    {
        try
        {
            await stream.CommitChanges(commitId, cancellationToken).ConfigureAwait(false);
        }
        catch (DuplicateCommitException ex)
        {
            _logger.LogError(ex, "{message}", ex.Message);
            await stream.ClearChanges().ConfigureAwait(false);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "{message}", ex.Message);
            throw new PersistenceException(ex.Message, ex);
        }
    }
}