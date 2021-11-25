// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the base class for domain event handlers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Events
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the base class for domain event handlers.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of event to handle.</typeparam>
    public abstract class EventHandlerBase<T> : IHandleEvents<T>
        where T : BaseEvent
    {
        private readonly ILogger<EventHandlerBase<T>> _logger;

        protected EventHandlerBase(ILogger<EventHandlerBase<T>> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task Handle(
            T @event,
            IMessageHeaders headers,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _logger.LogDebug("Handling {typeName} event.", typeof(T).Name);
            if (@event is DomainEvent domainEvent)
            {
                _logger.LogDebug("Correlation ID {correlationId}", domainEvent.CorrelationId);
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (await VerifyUserToken(headers.UserToken).ConfigureAwait(false))
            {
                await HandleInternal(@event, headers, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual bool CanHandle(Type type) => type != null && typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

        /// <summary>
        /// Verifies that the user token passed with the command is valid and authorizes the action.
        /// </summary>
        /// <param name="token">The user token. The token may be <c>null</c> if no token is sent with the message.</param>
        /// <returns><c>true</c> is the authentication is successful, otherwise <c>false</c>.</returns>
        protected virtual Task<bool> VerifyUserToken(string? token) => Task.FromResult(true);

        /// <summary>
        /// Handles the passed event.
        /// </summary>
        /// <param name="domainEvent">The <see cref="DomainEvent"/> to handle.</param>
        /// <param name="headers">The headers associated with the <see cref="DomainEvent"/> message.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the async operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the handling operation.</returns>
        protected abstract Task HandleInternal(T domainEvent, IMessageHeaders headers, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Disposes the handler.
        /// </summary>
        /// <param name="isDisposing">True if the dispose was raised by a dispose operation, otherwise false.</param>
        protected virtual void Dispose(bool isDisposing) { }
    }
}