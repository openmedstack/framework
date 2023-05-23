// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownUserException.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the exception when a user id or claim cannot be verified.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Exceptions;

using System;

/// <summary>
/// Defines the exception when a user id or claim cannot be verified.
/// </summary>
public class UnknownUserException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownUserException"/> class.
    /// </summary>
    public UnknownUserException() : base("Unknown user id.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownUserException"/> class.
    /// </summary>
    /// <param name="id">The <see cref="string">id</see> which could not be verified.</param>
    public UnknownUserException(string id)
        : base(id + " is an unknown user id.")
    {
    }
}