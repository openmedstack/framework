// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalAuthenticationError.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the exception when an external user authentication fails.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Exceptions;

using System;

/// <summary>
/// Defines the exception when an external user authentication fails.
/// </summary>
public class ExternalAuthenticationError : Exception
{
    public ExternalAuthenticationError(Exception innerException)
        : base("External authentication failed", innerException)
    {
    }
}
