// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISubscribeEvents.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the domain event subscription interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Events
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the domain event subscription interface.
    /// </summary>
    public interface ISubscribeEvents<out T> : IDisposable
        where T : BaseEvent
    {
        /// <summary>
        /// Subscribes to the defined message type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="BaseEvent"/> to subscribe to.</typeparam>
        /// <param name="handler">The subscription handler.</param>
        /// <returns>An <see cref="IDisposable"/> subscription token.</returns>
        Task<IDisposable> Subscribe(IHandleEvents<T> handler);
    }
}
