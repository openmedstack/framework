// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IService.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the service interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;

    /// <summary>
    /// Defines the service interface.
    /// </summary>
    public interface IService : IObservable<BaseEvent>, IDisposable
    {
        /// <summary>
        /// <para>Starts the service.</para>
        /// <para>This is called internally and does not need to be called when creating from a <see cref="Chassis"/>.</para>
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the start task.</param>
        /// <returns>A <see cref="Task"/> encapsulating the start operation.</returns>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Sends the command to the service service bus end point.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="DomainCommand"/> to send.</typeparam>
        /// <param name="msg">The <see cref="DomainCommand"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the send operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the send task.</returns>
        Task<CommandResponse> Send<T>(T msg, CancellationToken cancellationToken = default) where T : DomainCommand;

        /// <summary>
        /// Publishes the event to the service service bus end point.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="BaseEvent"/> to publish.</typeparam>
        /// <param name="msg">The <see cref="BaseEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the publish operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the publish task.</returns>
        Task Publish<T>(T msg, CancellationToken cancellationToken = default) where T : BaseEvent;

        /// <summary>
        /// Resolves an instance of <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the instance to resolve.</typeparam>
        /// <returns>An instance of <see cref="T"/>.</returns>
        T Resolve<T>() where T : class;
    }
}