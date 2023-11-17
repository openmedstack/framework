namespace OpenMedStack.Domain;

using System.Collections.Generic;
using OpenMedStack.Commands;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;

/// <summary>
/// Defines the saga interface.
/// </summary>
public interface ISaga
{
    /// <summary>
    /// Gets the saga identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the saga version.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Gets the event stream for the saga
    /// </summary>
    IEventStream Stream { get; }

    /// <summary>
    /// Transitions the saga to the next state based on the passed <see cref="BaseEvent"/>.
    /// </summary>
    /// <param name="message">The <see cref="BaseEvent"/> to handle.</param>
    void Transition(BaseEvent message);
}
