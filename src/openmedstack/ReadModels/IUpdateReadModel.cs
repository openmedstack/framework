// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateReadModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Marker interface. For internal reflection use only.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.ReadModels;

using System.Threading;
using System.Threading.Tasks;
using OpenMedStack.Events;

/// <summary>
/// Marker interface. For internal reflection use only.
/// </summary>
public interface IUpdateReadModel { }

/// <summary>
/// Defines the interface for read model updates.
/// </summary>
/// <typeparam name="T">The <see cref="BaseEvent"/> to get update information from.</typeparam>
public interface IUpdateReadModel<in T> : IUpdateReadModel
    where T : BaseEvent
{
    /// <summary>
    /// Updates the read model.
    /// </summary>
    /// <param name="domainEvent">The <see cref="BaseEvent"/> to update from.</param>
    /// <param name="headers">The message headers.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the update <see cref="Task"/>.</param>
    /// <returns>A <see cref="Task"/> encapsulating the update operation.</returns>
    Task Update(T domainEvent, IMessageHeaders headers, CancellationToken cancellationToken = default);
}