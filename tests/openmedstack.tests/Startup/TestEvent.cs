namespace OpenMedStack.Tests.Startup
{
    using System;
    using OpenMedStack.Events;

    public class TestEvent : BaseEvent
    {
        /// <inheritdoc />
        public TestEvent(string source, DateTimeOffset timeStamp, string? correlationId = null) : base(source, timeStamp, correlationId)
        {
        }
    }
}