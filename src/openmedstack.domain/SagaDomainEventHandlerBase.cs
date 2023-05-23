// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaDomainEventHandlerBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the abstract base class for handling <see cref="DomainEvent" /> in a saga.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Events;

/// <summary>
/// Defines the abstract base class for handling <see cref="DomainEvent"/> in a saga.
/// </summary>
/// <typeparam name="TSaga">The <see cref="ISaga"/> which will handle the event.</typeparam>
/// <typeparam name="TBaseEvent">The <see cref="DomainEvent"/> to handle.</typeparam>
public abstract class SagaDomainEventHandlerBase<TSaga, TBaseEvent> : IHandleEvents<TBaseEvent>
    where TSaga : class, ISaga
    where TBaseEvent : DomainEvent
{
    private readonly ISagaRepository _sagaRepository;
    private readonly ILogger<SagaDomainEventHandlerBase<TSaga, TBaseEvent>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaDomainEventHandlerBase{TSaga,TDomainEvent}"/> class.
    /// </summary>
    /// <param name="sagaRepository"></param>
    /// <param name="logger">The logger.</param>
    protected SagaDomainEventHandlerBase(ISagaRepository sagaRepository, ILogger<SagaDomainEventHandlerBase<TSaga, TBaseEvent>> logger)
    {
        _sagaRepository = sagaRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(TBaseEvent domainEvent, IMessageHeaders headers, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling saga notification of type " + typeof(TBaseEvent).FullName);

        var newMessage = await BeforeHandle(domainEvent, headers, cancellationToken).ConfigureAwait(false);
        if (newMessage.CorrelationId == null)
        {
            return;
        }
        var saga = await _sagaRepository.GetById<TSaga>(newMessage.CorrelationId).ConfigureAwait(false);

        _logger.LogDebug($"Transitioning with {typeof(TBaseEvent).Name}");
        saga.Transition(newMessage);

        _logger.LogDebug("Saving saga.");
        await _sagaRepository.Save(saga).ConfigureAwait(false);
        await AfterHandle(newMessage, headers, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs an action before the <see cref="DomainEvent"/> is handled by the <see cref="ISaga"/>.
    /// </summary>
    /// <param name="message">The <see cref="DomainEvent"/> to handle.</param>
    /// <param name="headers">The <see cref="IMessageHeaders"/> related to the <see cref="DomainEvent"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the async operation.</param>
    /// <returns>The handling operation as a <see cref="Task"/>.</returns>
    protected virtual Task<TBaseEvent> BeforeHandle(TBaseEvent message, IMessageHeaders headers, CancellationToken cancellationToken) => Task.FromResult(message);

    /// <summary>
    /// Performs an action after the <see cref="DomainEvent"/> is handled by the <see cref="ISaga"/>.
    /// </summary>
    /// <param name="message">The <see cref="DomainEvent"/> to handle.</param>
    /// <param name="headers">The <see cref="IMessageHeaders"/> related to the <see cref="DomainEvent"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the async operation.</param>
    /// <returns>The handling operation as a <see cref="Task"/>.</returns>
    protected virtual Task AfterHandle(TBaseEvent message, IMessageHeaders headers, CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    public bool CanHandle(Type type) => typeof(TBaseEvent).IsAssignableFrom(type);
}