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

internal sealed class DefaultEventStoreRepository : IRepository
{
    private const string AggregateTypeHeader = "AggregateType";
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<DefaultEventStoreRepository> _logger;
    private readonly IProvideTenant _tenantId;
    private readonly ICommitEvents _eventStore;
    private readonly IAccessSnapshots _snapshots;
    private readonly IConstructAggregates _factory;

    public DefaultEventStoreRepository(
        IProvideTenant tenantId,
        ICommitEvents eventStore,
        IAccessSnapshots snapshots,
        IConstructAggregates factory,
        ILoggerFactory logger)
    {
        _tenantId = tenantId;
        _eventStore = eventStore;
        _snapshots = snapshots;
        _factory = factory;
        _loggerFactory = logger;
        _logger = _loggerFactory.CreateLogger<DefaultEventStoreRepository>();
    }

    public Task<TAggregate> GetById<TAggregate>(string id, CancellationToken cancellationToken)
        where TAggregate : class, IAggregate =>
        GetById<TAggregate>(id, int.MaxValue, cancellationToken);

    public async Task<TAggregate> GetById<TAggregate>(string id, int version, CancellationToken cancellationToken)
        where TAggregate : class, IAggregate
    {
        var snapshot = await GetSnapshot(_tenantId.GetTenantName(), id, version, cancellationToken)
            .ConfigureAwait(false);
        var stream = await OpenStream(_tenantId.GetTenantName(), id, version, snapshot, cancellationToken)
            .ConfigureAwait(false);
        var aggregate = GetAggregate<TAggregate>(snapshot, stream);
        ApplyEventsToAggregate(version, stream, aggregate);

        _logger.LogDebug(
            "Loaded aggregate of type {TypeName} with id {Id} at version {Version}",
            typeof(TAggregate).Name,
            id,
            aggregate.Version);

        return (TAggregate)aggregate;
    }

    public async Task Save(
        IAggregate aggregate,
        Action<IDictionary<string, object>>? updateHeaders = null,
        CancellationToken cancellationToken = default)
    {
        var commitId = Guid.NewGuid();
        var headers = PrepareHeaders(aggregate, updateHeaders);

        var bucketId = _tenantId.GetTenantName();
        var stream = await PrepareStream(bucketId, aggregate, headers, cancellationToken)
            .ConfigureAwait(false);
        await _eventStore.Commit(stream, commitId, cancellationToken).ConfigureAwait(false);
        aggregate.ClearUncommittedEvents();

        _logger.LogDebug(
            "Saved aggregate of type {Type} with id {Id} at version {Version}",
            aggregate.GetType(),
            aggregate.Id,
            aggregate.Version);
    }

    private static void ApplyEventsToAggregate(int versionToLoad, IEventStream stream, IAggregate aggregate)
    {
        if (versionToLoad != 0 && aggregate.Version >= versionToLoad)
        {
            return;
        }

        foreach (var @event in stream.CommittedEvents.Select(x => x.Body).OfType<DomainEvent>().ToArray())
        {
            aggregate.ApplyEvent(@event);
        }
    }

    private IAggregate GetAggregate<TAggregate>(ISnapshot? snapshot, IEventStream stream)
    {
        var snapshot1 = snapshot?.Payload as IMemento;
        return _factory.Build(typeof(TAggregate), stream.StreamId, snapshot1);
    }

    private Task<ISnapshot?> GetSnapshot(
        string bucketId,
        string id,
        int version,
        CancellationToken cancellationToken) =>
        _snapshots.GetSnapshot(bucketId, id, version, cancellationToken);

    private async Task<IEventStream> OpenStream(
        string bucketId,
        string id,
        int version,
        ISnapshot? snapshot,
        CancellationToken cancellationToken)
    {
        var eventStream = snapshot == null
            ? OptimisticEventStream.Create(
                bucketId,
                id,
                _eventStore,
                0,
                version,
                _loggerFactory.CreateLogger<OptimisticEventStream>(),
                cancellationToken)
            : OptimisticEventStream.Create(
                snapshot,
                _eventStore,
                version,
                _loggerFactory.CreateLogger<OptimisticEventStream>(),
                cancellationToken);

        return await eventStream.ConfigureAwait(false);
    }

    private async Task<IEventStream> PrepareStream(
        string bucketId,
        IAggregate aggregate,
        Dictionary<string, object> headers,
        CancellationToken cancellationToken)
    {
        var stream = await OpenStream(bucketId, aggregate.Id, aggregate.Version, null, cancellationToken)
            .ConfigureAwait(false);

        foreach (var (key, value) in headers)
        {
            stream.Add(key, value);
        }

        foreach (var uncommittedEvent in aggregate.GetUncommittedEvents().ToArray())
        {
            stream.Add(new EventMessage(uncommittedEvent));
        }

        return stream;
    }

    private static Dictionary<string, object> PrepareHeaders(
        IAggregate aggregate,
        Action<IDictionary<string, object>>? updateHeaders = null)
    {
        var dictionary = new Dictionary<string, object> { { AggregateTypeHeader, aggregate.GetType().FullName! } };
        updateHeaders?.Invoke(dictionary);

        return dictionary;
    }

//    private bool ThrowOnConflict(IEventStream stream, int skip)
//    {
//        var committedEvents = stream.CommittedEvents.Skip(skip).Select(x => x.Body).OfType<BaseEvent>();
//        return _conflictDetector.ConflictsWith(
//            stream.UncommittedEvents.Select(x => x.Body).OfType<BaseEvent>(),
//            committedEvents);
//    }
}
