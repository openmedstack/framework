// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommitDispatcher.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
//   //   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CommitDispatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.NEventStore.Abstractions;

public class ReadModelCommitDispatcher : IReadModelCommitDispatcher
{
    private readonly ILogger<IReadModelCommitDispatcher> _logger;
    private readonly IReadModelUpdater _updater;
    private bool _isDisposed;

    public ReadModelCommitDispatcher(ILogger<IReadModelCommitDispatcher> logger, IReadModelUpdater readModelUpdater)
    {
        _logger = logger;
        _updater = readModelUpdater;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _isDisposed = true;
    }

    /// <inheritdoc />
    public async Task<HandlingResult> Dispatch(ICommit commit, CancellationToken cancellationToken)
    {
        if (_isDisposed)
        {
            _logger.LogWarning("Dispatching commits with disposed dispatcher");
            return HandlingResult.Stop;
        }

        try
        {
            _logger.LogInformation("Updating read models for commit {commitId}", commit.CommitId.ToString());
            var updated = await _updater.Update(commit).ConfigureAwait(false);

            return updated ? HandlingResult.MoveToNext : HandlingResult.Retry;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{error}", exception.Message);
            return HandlingResult.Retry;
        }
    }
}
