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
            var evt = new TestEvent(Id, Version + 1, DateTimeOffset.UtcNow);
            RaiseEvent(evt);
        }

        /// <inheritdoc />
        protected override void ApplySnapshot(EmptyMemento snapshot)
        {
        }

        /// <inheritdoc />
        public override EmptyMemento GetSnapshot() => new(Id, Version);

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TestEvent _)
#pragma warning restore IDE0051 // Remove unused private members
        {
            EventRaised = true;
        }
    }
}