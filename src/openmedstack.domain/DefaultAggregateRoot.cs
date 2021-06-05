namespace OpenMedStack.Domain
{
    public abstract class DefaultAggregateRoot : AggregateRootBase<EmptyMemento>
    {
        /// <inheritdoc />
        protected DefaultAggregateRoot(string id, IMemento? snapshot) : base(id, snapshot)
        {
        }

        /// <inheritdoc />
        protected override sealed EmptyMemento CreateSnapshot() => new();

        /// <inheritdoc />
        protected override sealed void ApplySnapshot(EmptyMemento snapshot)
        {
            // Deliberately empty.
        }
    }
}