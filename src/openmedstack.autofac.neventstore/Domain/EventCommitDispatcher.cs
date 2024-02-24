namespace OpenMedStack.Autofac.NEventstore.Domain;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;

public class EventCommitDispatcher(
    ILogger<EventCommitDispatcher> logger,
    Func<IPublishEvents> bus)
    : IEventCommitDispatcher
{
    private readonly ConcurrentDictionary<Type, MethodInfo> _eventPublishMethods =
        new();

    private readonly MethodInfo _eventBusPublishMethod = typeof(IPublishEvents).GetMethod("Publish")!;
    private bool _isDisposed;

    public async Task<HandlingResult> Dispatch(ICommit commit, CancellationToken cancellationToken)
    {
        if (_isDisposed)
        {
            logger.LogWarning("Dispatching commits with disposed dispatcher");
            return HandlingResult.Stop;
        }

        if (commit.Headers.ContainsKey("SagaType") || commit.Events.Count <= 0)
        {
            return HandlingResult.MoveToNext;
        }

        try
        {
            var token = commit.CheckpointToken;
            var eventBus = bus();
            var events = (from evt in commit.Events
                          where evt.Body is DomainEvent
                          let eventHeaders =
                              new Dictionary<string, object>(evt.Headers) { [Constants.CommitSequence] = token }
                          let method =
                              _eventPublishMethods.GetOrAdd(evt.Body.GetType(), ValueFactory)
                          let result = method.Invoke(
                              eventBus,
                              new[] { evt.Body, eventHeaders, CancellationToken.None })
                          select (Task)result).ToArray();

            logger.LogDebug(
                "Publishing {Count} events for commit {CommitId}",
                events.Length,
                commit.CommitId);

            await Task.WhenAll(events).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{Error}", exception.Message);
            return HandlingResult.Retry;
        }

        return HandlingResult.MoveToNext;
    }

    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "Generic method available")]
    private MethodInfo ValueFactory(Type t) => _eventBusPublishMethod.MakeGenericMethod(t);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _isDisposed = true;
    }
}
