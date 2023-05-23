// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the interface for read model objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.ReadModels;

/// <summary>
/// Defines the interface for read model objects.
/// </summary>
public interface IReadModel
{
    /// <summary>
    /// Gets the version number of the latest update.
    /// </summary>
    int Version { get; }
}
