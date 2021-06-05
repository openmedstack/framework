using System;

namespace OpenMedStack.Domain
{
    public class HandlerForDomainEventNotFoundException : Exception
    {
        public HandlerForDomainEventNotFoundException(string message)
            : base(message)
        {
        }
    }
}