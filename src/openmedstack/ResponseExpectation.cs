// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseExpectation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the enumeration of response expectations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

/// <summary>
/// Defines the enumeration of response expectations.
/// </summary>
public enum ResponseExpectation
{
    /// <summary>
    /// A response is always expected.
    /// </summary>
    Always = 0,

    /// <summary>
    /// No response is expected.
    /// </summary>
    Never = 1,

    /// <summary>
    /// Only error responses are expected.
    /// </summary>
    Error = 2
}
