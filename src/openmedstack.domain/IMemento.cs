namespace OpenMedStack.Domain;

/// <summary>
/// Defines the memento interface.
/// </summary>
public interface IMemento
{
    /// <summary>
    /// Gets the aggregate identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the aggregate version.
    /// </summary>
    int Version { get; }
}
