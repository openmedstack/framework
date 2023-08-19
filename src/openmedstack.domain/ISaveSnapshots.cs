// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveSnapshots.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the interface for persisting snapshots.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

using System;

/// <summary>
/// Defines the interface for persisting snapshots.
/// </summary>
public interface ISaveSnapshots
{
    /// <summary>
    /// Saves the snapshot of the given aggregate root.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of snapshot.</typeparam>
    /// <param name="aggregateRoot">The aggregate to snapshot.</param>
    bool Save<T>(AggregateRootBase<T> aggregateRoot) where T : IMemento;
}
