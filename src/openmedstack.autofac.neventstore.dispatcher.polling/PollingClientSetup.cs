namespace OpenMedStack.Autofac.NEventStore.Dispatcher.Polling;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Domain;
using OpenMedStack.NEventStore.Abstractions;

internal class PollingClientSetup<TCheckpointTracker, TCommitDispatcher> : IBootstrapSystem
    where TCheckpointTracker : class, ITrackCheckpoints where TCommitDispatcher : class, ICommitDispatcher
{
    private readonly ICommitDispatcher _dispatcher;
    private readonly IProvideTenant _tenantProvider;
    private readonly TCheckpointTracker _checkpointTracker;
    private readonly ILogger<PollingClientSetup<TCheckpointTracker, TCommitDispatcher>> _logger;
    private readonly AsyncPollingClient _pollingClient2;
    private int _errorCount;

    public PollingClientSetup(
        IManagePersistence persistence,
        TCommitDispatcher dispatcher,
        IProvideTenant tenantProvider,
        TCheckpointTracker checkpointTracker,
        ILoggerFactory loggerFactory,
        TimeSpan pollingInterval = default)
    {
        _dispatcher = dispatcher;
        _tenantProvider = tenantProvider;
        _checkpointTracker = checkpointTracker;
        _logger = loggerFactory.CreateLogger<PollingClientSetup<TCheckpointTracker, TCommitDispatcher>>();
        _pollingClient2 = new AsyncPollingClient(
            persistence,
            HandleCommit,
            loggerFactory.CreateLogger<AsyncPollingClient>(),
            pollingInterval == default ? TimeSpan.FromMilliseconds(500) : pollingInterval);
    }

    /// <inheritdoc />
    public uint Order { get; } = 10;

    /// <inheritdoc />
    public async Task Setup(CancellationToken cancellationToken)
    {
        var tenantName = _tenantProvider.GetTenantName();
        await _pollingClient2.StartFrom(_checkpointTracker, tenantName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task Shutdown(CancellationToken cancellationToken)
    {
        _pollingClient2.Stop();
        _pollingClient2.Dispose();
        _dispatcher.Dispose();
        return Task.CompletedTask;
    }

    private async Task<HandlingResult> HandleCommit(ICommit commit, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatcher.Dispatch(commit, cancellationToken).ConfigureAwait(false);
            _errorCount = 0;
            return result;
        }
        catch (Exception ex)
        {
            _errorCount++;
            _logger.LogError(ex, "{Error}", ex.Message);
            return _errorCount > 3 ? HandlingResult.Stop : HandlingResult.Retry;
        }
    }
}
