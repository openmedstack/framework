namespace OpenMedStack.Framework.IntegrationTests
{
    using System;
    using OpenMedStack.Events;

    public class TestEvent : DomainEvent
    {
        /// <inheritdoc />
        public TestEvent() : base(Guid.NewGuid().ToString(), 1, DateTimeOffset.Now)
        {
        }
    }
}