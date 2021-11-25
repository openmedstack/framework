namespace openmedstack.masstransit.tests
{
    using System;
    using OpenMedStack.Events;

    internal class TestEvent : DomainEvent
    {
        /// <inheritdoc />
        public TestEvent(string source, int version, DateTimeOffset timeStamp)
            : base(source, version, timeStamp)
        {
        }
    }
}