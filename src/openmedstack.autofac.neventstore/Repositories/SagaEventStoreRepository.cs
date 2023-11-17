namespace OpenMedStack.Autofac.NEventstore.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Domain;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;

public class SagaEventStoreRepository : ISagaRepository
{
    private readonly IProvideTenant _tenantProvider;
    private readonly ICommitEvents _eventStore;
    private readonly IConstructSagas _factory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<SagaEventStoreRepository> _logger;

    public SagaEventStoreRepository(
        IProvideTenant tenantProvider,
        ICommitEvents eventStore,
        IConstructSagas factory,
        ILoggerFactory loggerFactory)
    {
        _tenantProvider = tenantProvider;
        _eventStore = eventStore;
        _factory = factory;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<SagaEventStoreRepository>();
    }

    public async Task<TSaga> GetById<TSaga>(string sagaId, CancellationToken cancellationToken = default)
        where TSaga : ISaga
    {
        var eventStream = await OpenStream(
                _tenantProvider.GetTenantName(),
                sagaId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return BuildSaga<TSaga>(sagaId, eventStream);
    }

    public async Task Save(
        ISaga saga,
        Action<IDictionary<string, object>>? updateHeaders,
        CancellationToken cancellationToken)
    {
        if (saga == null)
        {
            throw new ArgumentNullException(nameof(saga), "Saga cannot be null");
        }

        await Persist(saga.Stream, Guid.NewGuid(), cancellationToken).ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private async Task<IEventStream> OpenStream(
        string bucketId,
        string sagaId,
        int maxVersion = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        IEventStream eventStream;
        try
        {
            eventStream = await OptimisticEventStream.Create(
                    bucketId,
                    sagaId,
                    _eventStore,
                    0,
                    maxVersion,
                    _loggerFactory.CreateLogger<OptimisticEventStream>(),
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (StreamNotFoundException)
        {
            eventStream = OptimisticEventStream.Create(bucketId, sagaId);
        }

        return eventStream;
    }

    private TSaga BuildSaga<TSaga>(string sagaId, IEventStream stream) where TSaga : ISaga
    {
        var saga = _factory.Build<TSaga>(sagaId, stream);
        foreach (var message in stream.CommittedEvents.Select(x => x.Body).OfType<BaseEvent>().ToArray())
        {
            saga.Transition(message);
        }

        return saga;
    }

    private async Task Persist(IEventStream stream, Guid commitId, CancellationToken cancellationToken)
    {
        var commit = await _eventStore.Commit(stream, commitId, cancellationToken).ConfigureAwait(false);
        if (commit != null)
        {
            stream.SetPersisted(commit.CommitSequence);
        }
    }
}
