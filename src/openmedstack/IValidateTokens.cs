// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidateTokens.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the token validation interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System.Security.Principal;

/// <summary>
/// Defines the token validation interface
/// </summary>
public interface IValidateTokens
{
    /// <summary>
    /// Validates the given token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>If validation succeeds then returns the token <see cref="IPrincipal"/>, otherwise null.</returns>
    IdentityToken? Validate(string? token);
}
