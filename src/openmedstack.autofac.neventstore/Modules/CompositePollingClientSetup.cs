namespace OpenMedStack.Autofac.NEventstore.Modules
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Autofac.NEventstore.Domain;
    using OpenMedStack.Domain;
    using OpenMedStack.NEventStore;
    using OpenMedStack.NEventStore.PollingClient;
    using OpenMedStack.NEventStore.Persistence;

    internal class CompositePollingClientSetup : IBootstrapSystem
    {
        private readonly IProvideTenant _tenantProvider;
        private readonly ILogger<AsyncPollingClient> _logger;
        private readonly AsyncPollingClient _pollingClient2;
        private readonly (ITrackCheckpoints checkpointTracker, ICommitDispatcher commitDispatcher)[] _dispatchers;
        private readonly CompositeCheckpointTracker _tracker;
        private byte _errorCount;

        public CompositePollingClientSetup(
            IPersistStreams persistence,
            IProvideTenant tenantProvider,
            ILogger<AsyncPollingClient> logger,
            TimeSpan pollingInterval = default,
            params (ITrackCheckpoints checkpointTracker, ICommitDispatcher commitDispatcher)[] dispatchers)
        {
            _tenantProvider = tenantProvider;
            _logger = logger;
            _dispatchers = dispatchers;
            _tracker = new CompositeCheckpointTracker(dispatchers.Select(x => x.checkpointTracker).ToArray());
            _pollingClient2 = new AsyncPollingClient(
                persistence,
                HandleCommit,
                logger,
                pollingInterval == default ? TimeSpan.FromMilliseconds(500) : pollingInterval);
        }

        private async Task<PollingClient2.HandlingResult> HandleCommit(
            ICommit commit,
            CancellationToken cancellationToken)
        {
            var tasks = _dispatchers.Select(
                async tuple =>
                {
                    try
                    {
                        var (checkpointTracker, commitDispatcher) = tuple;
                        if (commit.CheckpointToken <= await checkpointTracker.GetLatest().ConfigureAwait(false))
                        {
                            return PollingClient2.HandlingResult.MoveToNext;
                        }

                        return await commitDispatcher.Dispatch(commit, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _errorCount++;
                        _logger.LogError("{error}", ex.Message);
                        return _errorCount >= 3
                            ? PollingClient2.HandlingResult.Stop
                            : PollingClient2.HandlingResult.Retry;
                    }
                });
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            if (results.Any(x => x == PollingClient2.HandlingResult.Stop))
            {
                return PollingClient2.HandlingResult.Stop;
            }

            return results.Any(x => x == PollingClient2.HandlingResult.Retry)
                ? PollingClient2.HandlingResult.Retry
                : PollingClient2.HandlingResult.MoveToNext;
        }

        /// <inheritdoc />
        public uint Order { get; } = 10;

        /// <inheritdoc />
        public async Task Setup(CancellationToken cancellationToken)
        {
            var tenantName = _tenantProvider.GetTenantName();
            await _pollingClient2.StartFrom(_tracker, tenantName, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task Shutdown(CancellationToken cancellationToken)
        {
            _pollingClient2.Stop();
            _pollingClient2.Dispose();
            foreach (var (checkpointTracker, commitDispatcher) in _dispatchers)
            {
                checkpointTracker.TryDispose();
                commitDispatcher.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}
