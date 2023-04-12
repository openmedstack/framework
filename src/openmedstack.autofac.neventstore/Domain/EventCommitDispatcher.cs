namespace OpenMedStack.Autofac.NEventstore.Domain
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Events;
    using NEventStore;
    using NEventStore.PollingClient;

    public class EventCommitDispatcher : IEventCommitDispatcher
    {
        private readonly ConcurrentDictionary<Type, MethodInfo> _eventPublishMethods =
            new();

        private readonly ILogger<EventCommitDispatcher> _logger;
        private readonly Func<IPublishEvents> _eventBus;
        private readonly MethodInfo _eventBusPublishMethod;
        private bool _isDisposed;

        public EventCommitDispatcher(
            ILogger<EventCommitDispatcher> logger,
            Func<IPublishEvents> eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
            _eventBusPublishMethod = typeof(IPublishEvents).GetMethod("Publish")!;
        }

        public async Task<PollingClient2.HandlingResult> Dispatch(ICommit commit, CancellationToken cancellationToken)
        {
            if (_isDisposed)
            {
                _logger.LogWarning("Dispatching commits with disposed dispatcher");
                return PollingClient2.HandlingResult.Stop;
            }

            if (!commit.Headers.ContainsKey("SagaType") && commit.Events.Count > 0)
            {
                try
                {
                    var token = commit.CheckpointToken;
                    var eventBus = _eventBus();
                    var events = (from evt in commit.Events
                                  where evt.Body is DomainEvent
                                  let eventHeaders =
                                      new Dictionary<string, object>(evt.Headers) { [Constants.CommitSequence] = token }
                                  let method =
                                      _eventPublishMethods.GetOrAdd(
                                          evt.Body.GetType(),
                                          t => _eventBusPublishMethod.MakeGenericMethod(t))
                                  let result = method.Invoke(eventBus, new[] { evt.Body, eventHeaders, CancellationToken.None })
                                  select (Task)result).ToArray();

                    _logger.LogInformation("Publishing {count} events for commit {commitId}", events.Length, commit.CommitId);

                    await Task.WhenAll(events).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                    return PollingClient2.HandlingResult.Retry;
                }
            }

            return PollingClient2.HandlingResult.MoveToNext;
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}
