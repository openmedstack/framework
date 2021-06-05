// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICorrelate.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the correlation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    /// <summary>
    /// Defines the correlation interface.
    /// </summary>
    public interface ICorrelate
    {
        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        string? CorrelationId { get; }
    }
}