namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using OpenMedStack.Events;

/// <summary>
/// Defines the aggregate interface.
/// </summary>
public interface IAggregate : IDisposable
{
    /// <summary>
    /// Gets the aggregate identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the aggregate version.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Applies the event to the aggregate.
    /// </summary>
    /// <param name="event"></param>
    void ApplyEvent(DomainEvent @event);

    /// <summary>
    /// Gets the uncommitted events.
    /// </summary>
    /// <returns>The uncommitted events as an <see cref="IEnumerable{T}"/></returns>
    IEnumerable<DomainEvent> GetUncommittedEvents();

    /// <summary>
    /// Clears the uncommitted events.
    /// </summary>
    void ClearUncommittedEvents();

    /// <summary>
    /// Gets the aggregate snapshot.
    /// </summary>
    /// <returns></returns>
    IMemento GetSnapshot();
}
