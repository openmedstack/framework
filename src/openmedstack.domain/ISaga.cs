namespace OpenMedStack.Domain;

using System.Collections.Generic;
using OpenMedStack.Commands;
using OpenMedStack.Events;

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
    /// Transitions the saga to the next state based on the passed <see cref="BaseEvent"/>.
    /// </summary>
    /// <param name="message"></param>
    void Transition(BaseEvent message);

    /// <summary>
    /// Gets the uncommitted events.
    /// </summary>
    /// <returns>The uncommitted events as an <see cref="IEnumerable{T}"/></returns>
    IEnumerable<BaseEvent> GetUncommittedEvents();

    /// <summary>
    /// Clears the uncommitted events.
    /// </summary>
    void ClearUncommittedEvents();

    /// <summary>
    /// Gets the undispatched messages.
    /// </summary>
    /// <returns>The uncommitted messages as an <see cref="IEnumerable{T}"/></returns>
    IEnumerable<DomainCommand> GetUndispatchedMessages();

    /// <summary>
    /// Clears the undispatched messages.
    /// </summary>
    void ClearUndispatchedMessages();
}
