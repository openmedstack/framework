namespace OpenMedStack.Domain.Tests
{
    using System;
    using OpenMedStack.Events;

    internal class TestEvent : DomainEvent
    {
        /// <inheritdoc />
        public TestEvent(string source, int version, DateTimeOffset timeStamp, string? correlationId = null)
            : base(source, version, timeStamp, correlationId)
        {
        }
    }
}