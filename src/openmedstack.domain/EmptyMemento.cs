// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyMemento.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the EmptyMemento type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

/// <summary>
/// Defines the empty memento type. This can be used when an aggregate will not be hydrated from a memento.
/// </summary>
public class EmptyMemento : IMemento
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyMemento"/> class.
    /// </summary>
    /// <param name="id">The aggregate id.</param>
    /// <param name="version">The aggregate version.</param>
    public EmptyMemento(string id, int version)
    {
        Id = id;
        Version = version;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public int Version { get; }
}
