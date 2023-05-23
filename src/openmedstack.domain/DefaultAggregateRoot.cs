namespace OpenMedStack.Domain;

public abstract class DefaultAggregateRoot : AggregateRootBase<EmptyMemento>
{
    /// <inheritdoc />
    protected DefaultAggregateRoot(string id, IMemento? snapshot) : base(id, snapshot)
    {
    }

    /// <inheritdoc />
    public override sealed EmptyMemento GetSnapshot() => new(Id, Version);

    /// <inheritdoc />
    protected override sealed void ApplySnapshot(EmptyMemento snapshot)
    {
        // Deliberately empty.
    }
}
