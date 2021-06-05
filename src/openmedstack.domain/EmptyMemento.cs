// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyMemento.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the EmptyMemento type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain
{
    public class EmptyMemento : IMemento
    {
        public string Id { get; set; } = null!;

        public int Version { get; set; }
    }
}