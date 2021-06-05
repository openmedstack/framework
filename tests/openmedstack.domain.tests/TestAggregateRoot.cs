namespace OpenMedStack.Domain.Tests
{
    using System;

    internal class TestAggregateRoot : AggregateRootBase<EmptyMemento>
    {
        public bool EventRaised { get; private set; }

        /// <inheritdoc />
        public TestAggregateRoot(string id, IMemento? snapshot)
            : base(id, snapshot)
        {
        }

        public void SomeAction()
        {
            var evt = new TestEvent(Id, Version, DateTimeOffset.UtcNow);
            RaiseEvent(evt);
        }

        /// <inheritdoc />
        protected override void ApplySnapshot(EmptyMemento snapshot)
        {
        }

        /// <inheritdoc />
        protected override EmptyMemento CreateSnapshot() => new() { Id = Id, Version = Version };

        private void Apply(TestEvent evt)
        {
            EventRaised = true;
        }
    }
}