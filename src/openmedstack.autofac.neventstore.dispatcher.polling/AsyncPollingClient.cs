namespace OpenMedStack.Autofac.NEventstore.Modules;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Domain;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.PollingClient;

internal sealed class AsyncPollingClient : IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();
    private readonly Func<ICommit, CancellationToken, Task<HandlingResult>> _commitCallback;
    private readonly IPersistStreams _persistStreams;
    private readonly ILogger<AsyncPollingClient> _logger;
    private readonly TimeSpan _waitInterval;
    private Task? _pollingThread;
    private Func<IAsyncEnumerable<ICommit>>? _pollingFunc;
    private ITrackCheckpoints _checkpointToken = null!;
    private int _isPolling;
    private bool _isDisposed;

    public AsyncPollingClient(
        IPersistStreams persistStreams,
        Func<ICommit, CancellationToken, Task<HandlingResult>> callback,
        ILogger<AsyncPollingClient> logger,
        TimeSpan waitInterval = default)
    {
        _waitInterval = waitInterval == new TimeSpan() ? TimeSpan.FromMilliseconds(300.0) : waitInterval;
        _commitCallback = callback ?? throw new ArgumentNullException(nameof(callback), "Cannot use polling client without callback");
        _persistStreams = persistStreams ?? throw new ArgumentNullException(nameof(persistStreams), "PersistStreams cannot be null");
        _logger = logger;
    }

    public async Task ConfigurePollingFunction(string bucketId, CancellationToken cancellationToken = default)
    {
        if (_pollingThread != null)
        {
            throw new PollingClientException("Cannot configure when polling client already started polling");
        }

        var latest = await _checkpointToken.GetLatest().ConfigureAwait(false);
        _pollingFunc = () => _persistStreams.GetFrom(
                bucketId,
                latest,
                cancellationToken);
    }

    public async Task StartFrom(ITrackCheckpoints checkpointToken, string bucketId, CancellationToken cancellationToken = default)
    {
        if (_pollingThread != null)
        {
            throw new PollingClientException("Polling client already started");
        }

        _checkpointToken = checkpointToken;
        await ConfigurePollingFunction(bucketId, cancellationToken).ConfigureAwait(false);
        _pollingThread = InnerPollingLoop();
    }

    public void Stop()
    {
        _tokenSource.Cancel();
    }

    private async Task InnerPollingLoop()
    {
        if (_pollingFunc == null)
        {
            throw new InvalidOperationException($"Must call {nameof(ConfigurePollingFunction)} before polling");
        }

        await Task.Yield();
        while (!_tokenSource.IsCancellationRequested)
        {
            if (Interlocked.CompareExchange(ref _isPolling, 1, 0) == 0)
            {
                if (await InnerPoll().ConfigureAwait(false))
                {
                    break;
                }
            }

            await Task.Delay(_waitInterval, _tokenSource.Token).ConfigureAwait(false);
        }
    }

    private async Task<bool> InnerPoll()
    {
        await Task.Yield();
        if (_tokenSource.IsCancellationRequested)
        {
            return true;
        }

        try
        {
            await foreach (var commit in _pollingFunc!().ConfigureAwait(false))
            {
                if (_tokenSource.IsCancellationRequested)
                {
                    return true;
                }

                switch (await _commitCallback(commit,_tokenSource.Token).ConfigureAwait(false))
                {
                    case HandlingResult.Retry:
                        _logger.LogError("Commit callback ask retry for checkpointToken {commitCheckpoint} - last dispatched {checkpointToken}", commit.CheckpointToken, _checkpointToken);
                        continue;
                    case HandlingResult.Stop:
                        Stop();
                        return true;
                    case HandlingResult.MoveToNext:
                        await _checkpointToken.SetLatest(commit.CheckpointToken).ConfigureAwait(false);
                        continue;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{message}", ex.Message);
        }
        Interlocked.Exchange(ref _isPolling, 0);

        return false;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Stop();
        _isDisposed = true;
    }
}
