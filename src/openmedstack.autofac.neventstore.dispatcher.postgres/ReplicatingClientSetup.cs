namespace openmedstack.autofac.nevenstore.dispatcher.postgres;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Commands;
using OpenMedStack.Domain;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.PostgresClient;

internal class ReplicatingClientSetup : IBootstrapSystem
{
    private readonly IPublishEvents _eventPublisher;
    private readonly IRouteCommands _commandRouter;
    private readonly ILogger<ReplicatingClientSetup> _logger;
    private readonly DelegatePgPublicationClient _client;
    private int _errorCount;

    public ReplicatingClientSetup(
        IPublishEvents eventPublisher,
        IRouteCommands commandRouter,
        ISerialize serializer,
        ILogger<ReplicatingClientSetup> logger)
    {
        _eventPublisher = eventPublisher;
        _commandRouter = commandRouter;
        _logger = logger;
        _client = new DelegatePgPublicationClient("", serializer, HandleCommit);
    }

    /// <inheritdoc />
    public uint Order { get; } = 10;

    /// <inheritdoc />
    public async Task Setup(CancellationToken cancellationToken)
    {
        await _client.CreateSubscriptionSlot(cancellationToken).ConfigureAwait(false);
        await _client.Subscribe(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task Shutdown(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task<HandlingResult> HandleCommit(Type type, object value, CancellationToken cancellationToken)
    {
        try
        {
            if (typeof(BaseEvent).IsAssignableFrom(type))
            {
                await _eventPublisher.Publish((dynamic)value, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else if (typeof(DomainCommand).IsAssignableFrom(type))
            {
                CommandResponse response = await _commandRouter
                    .Send((dynamic)value, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if (response.FaultMessage != null)
                {
                    _errorCount++;
                    _logger.LogError(response.FaultMessage);
                    return _errorCount > 3 ? HandlingResult.Stop : HandlingResult.Retry;
                }
            }

            _errorCount = 0;
            return HandlingResult.MoveToNext;
        }
        catch (Exception ex)
        {
            _errorCount++;
            _logger.LogError(ex.Message);
            return _errorCount > 3 ? HandlingResult.Stop : HandlingResult.Retry;
        }
    }
}
