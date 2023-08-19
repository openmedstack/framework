namespace OpenMedStack.Domain;

using System;

public class InvalidTransitionException : Exception
{
    public InvalidTransitionException(string message) : base(message)
    {
    }
}