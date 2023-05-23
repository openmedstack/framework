namespace OpenMedStack.Autofac.NEventstore.Domain;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Commands;
using NEventStore;
using NEventStore.PollingClient;

public class CommandCommitDispatcher<TConfiguration> : ICommandCommitDispatcher
    where TConfiguration : DeploymentConfiguration
{
    private readonly ConcurrentDictionary<Type, MethodInfo> _commandSendMethods =
        new();

    private readonly ILogger<CommandCommitDispatcher<TConfiguration>> _logger;
    private readonly Func<IRouteCommands> _commandBus;
    private readonly MethodInfo _commandBusSendMethod;
    private readonly TConfiguration _configuration;
    private bool _isDisposed;

    public CommandCommitDispatcher(
        ILogger<CommandCommitDispatcher<TConfiguration>> logger,
        Func<IRouteCommands> commandBus,
        TConfiguration configuration)
    {
        Contract.Requires(commandBus != null);

        _configuration = configuration;
        _logger = logger;
        _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
        _commandBusSendMethod = typeof(IRouteCommands).GetMethod("Send") ?? throw new NullReferenceException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async Task<PollingClient2.HandlingResult> Dispatch(ICommit commit, CancellationToken cancellationToken)
    {
        if (_isDisposed)
        {
            _logger.LogWarning("Dispatching commits with disposed dispatcher");
            return PollingClient2.HandlingResult.Stop;
        }

        var commands = (from header in commit.Headers
                        where header.Key.StartsWith(Constants.UndispatchedMessageHeader)
                        select header).ToArray();
        if (commands.Length > 0)
        {
            using var tokenSource = new CancellationTokenSource(_configuration.Timeout);
            var commandBus = _commandBus();
            var commandHeaders = commit.Headers
                .Where(x => !x.Key.StartsWith(Constants.UndispatchedMessageHeader))
                .ToDictionary(x => x.Key, x => x.Value);
            var commandTasks = (from cmd in commands
                                let command = cmd.Value
                                let method =
                                    _commandSendMethods.GetOrAdd(
                                        command.GetType(),
                                        t => _commandBusSendMethod.MakeGenericMethod(t))
                                let result = method.Invoke(commandBus, new[] { command, commandHeaders, tokenSource.Token })
                                select (Task<CommandResponse>)result).ToArray();

            _logger.LogInformation($"Dispatched {commandTasks.Length} commands for commit {commit.CommitId}");
            try
            {
                var commandResults = await Task.WhenAll(commandTasks).ConfigureAwait(false);

                var faults = commandResults.Where(x => !string.IsNullOrWhiteSpace(x.FaultMessage))
                    .GroupBy(x => new { x.TargetAggregate, x.Version })
                    .ToArray();
                if (faults.Length > 0)
                {
                    _logger.LogError($"Failed to send {faults.Length} commands.");

                    foreach (var fault in faults)
                    {
                        _logger.LogError(
                            $"Target: {fault.Key.TargetAggregate}, Version: {fault.Key.Version}, Errors: {string.Join(Environment.NewLine, fault.Select(x => x.FaultMessage))}");
                    }

                    return PollingClient2.HandlingResult.Retry;
                }

                return PollingClient2.HandlingResult.MoveToNext;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return PollingClient2.HandlingResult.Retry;
            }
        }

        return PollingClient2.HandlingResult.MoveToNext;
    }
}