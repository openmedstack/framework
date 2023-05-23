// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPublishEvents.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the domain event publish interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Events;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines the domain event publish interface.
/// </summary>
public interface IPublishEvents
{
    /// <summary>
    /// Publishes the domain event.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see cref="BaseEvent"/>.</typeparam>
    /// <param name="message">The <see cref="BaseEvent"/> to publish.</param>
    /// <param name="headers">The message headers.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the publish <see cref="Task"/>.</param>
    /// <returns>A <see cref="Task"/> encapsulating the publish operation.</returns>
    Task Publish<T>(T message, IDictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
        where T : BaseEvent;
}
