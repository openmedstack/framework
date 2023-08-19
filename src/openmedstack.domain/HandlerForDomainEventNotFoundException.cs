using System;

namespace OpenMedStack.Domain;

/// <summary>
/// Defines the exception for when a handler for a domain event is not found.
/// </summary>
public class HandlerForDomainEventNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerForDomainEventNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public HandlerForDomainEventNotFoundException(string message)
        : base(message)
    {
    }
}
