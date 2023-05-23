// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyMemento.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the EmptyMemento type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

public class EmptyMemento : IMemento
{
    public EmptyMemento(string id, int version)
    {
        Id = id;
        Version = version;
    }

    public string Id { get; }

    public int Version { get; }
}
