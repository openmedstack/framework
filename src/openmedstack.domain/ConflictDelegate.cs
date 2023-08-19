namespace OpenMedStack.Domain;

using OpenMedStack.Events;

/// <summary>
/// Determines whether two events conflict.
/// </summary>
/// <typeparam name="TUncommitted">The uncommitted <see cref="BaseEvent"/> to compare.</typeparam>
/// <typeparam name="TCommitted">The committed <see cref="BaseEvent"/> to compare.</typeparam>
public delegate bool ConflictDelegate<in TUncommitted, in TCommitted>(
    TUncommitted uncommitted,
    TCommitted committed)
    where TUncommitted : BaseEvent where TCommitted : BaseEvent;
