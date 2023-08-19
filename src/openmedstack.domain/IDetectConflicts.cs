using System.Collections.Generic;

namespace OpenMedStack.Domain;

using OpenMedStack.Events;

/// <summary>
/// Defines the event stream conflict detection interface.
/// </summary>
public interface IDetectConflicts
{
    /// <summary>
    /// Registers a conflict
    /// </summary>
    /// <param name="handler">The conflict detection handlers.</param>
    /// <typeparam name="TUncommitted">The uncommitted <see cref="BaseEvent"/> to compare.</typeparam>
    /// <typeparam name="TCommitted">The committed <see cref="BaseEvent"/> to compare.</typeparam>
    void Register<TUncommitted, TCommitted>(ConflictDelegate<TUncommitted, TCommitted> handler)
        where TUncommitted : BaseEvent
        where TCommitted : BaseEvent;

    /// <summary>
    /// Determines whether the two event streams conflict.
    /// </summary>
    /// <param name="uncommittedEvents">The uncommitted <see cref="BaseEvent"/> to compare.</param>
    /// <param name="committedEvents">The committed <see cref="BaseEvent"/> to compare.</param>
    /// <returns></returns>
    bool ConflictsWith(IEnumerable<BaseEvent> uncommittedEvents, IEnumerable<BaseEvent> committedEvents);
}
