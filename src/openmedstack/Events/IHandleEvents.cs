// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHandleEvents.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the base interface for domain event handlers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the base interface for domain event handlers.
    /// </summary>
    public interface IHandleEvents : IDisposable
    {
        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the <see cref="Type"/> can be handled.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>True is the <see cref="Type"/> can be handled, otherwise false.</returns>
        bool CanHandle(Type type);
    }

    /// <summary>
    /// Defines the generic domain event handler interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainEvent"/> to handle.</typeparam>
    public interface IHandleEvents<in T> : IHandleEvents
        where T : BaseEvent
    {
        /// <summary>
        /// Handles the passed event.
        /// </summary>
        /// <param name="domainEvent">The <see cref="DomainEvent"/> to handle.</param>
        /// <param name="headers">The headers associated with the <see cref="DomainEvent"/> message.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the async operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the handling operation.</returns>
        Task Handle(T domainEvent, IMessageHeaders headers, CancellationToken cancellationToken = default);
    }
}