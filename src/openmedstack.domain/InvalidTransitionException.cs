namespace OpenMedStack.Domain;

using System;

/// <summary>
/// Defines the invalid transaction exception.
/// </summary>
public class InvalidTransitionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidTransitionException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InvalidTransitionException(string message) : base(message)
    {
    }
}
