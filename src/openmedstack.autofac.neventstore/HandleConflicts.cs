namespace OpenMedStack.Autofac.NEventstore;

/// <summary>
/// Defines the conflict handler delegate.
/// </summary>
/// <typeparam name="TCommitted">The committed event <see cref="System.Type"/>.</typeparam>
/// <typeparam name="TUncommited">The uncommitted event <see cref="System.Type"/>.</typeparam>
/// <param name="committed">The committed event.</param>
/// <param name="uncommited">The uncommitted event.</param>
/// <returns>True if the conflict has been handled, otherwise false.</returns>
public delegate bool HandleConflicts<in TCommitted, in TUncommited>(TCommitted committed, TUncommited uncommited);