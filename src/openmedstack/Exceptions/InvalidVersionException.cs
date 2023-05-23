// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidVersionException.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the exception for invalid version of a message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Exceptions;

using System;

/// <summary>
/// Defines the exception for invalid version of a message.
/// </summary>
public class InvalidVersionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidVersionException"/> class.
    /// </summary>
    /// <param name="versionNumber">The actual version received.</param>
    /// <param name="expected">The expected version.</param>
    public InvalidVersionException(int versionNumber, int expected)
        : base($"Version number is {versionNumber}. Version {expected} was expected.")
    {
    }
}