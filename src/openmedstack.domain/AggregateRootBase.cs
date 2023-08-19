// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateRootBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the AggregateRootBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using OpenMedStack.Events;

/// <summary>
/// Defines the abstract aggregate root with default implementations.
/// </summary>
/// <typeparam name="TMemento">The <see cref="Type"/> of <see cref="IMemento"/> to create or use for rehydration.</typeparam>
public abstract class AggregateRootBase<TMemento> : IAggregate, IEquatable<IAggregate> where TMemento : IMemento
{
    private readonly ICollection<DomainEvent> _uncommittedEvents = new LinkedList<DomainEvent>();
    private readonly IDispatchEvents _registeredRoutes;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootBase{TMemento}"/> class.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="snapshot">The optional snapshot for rehydration.</param>
    protected AggregateRootBase(string id, IMemento? snapshot)
    {
        _registeredRoutes = new ConventionEventRouter(true, this);
        Id = id;
        Version = 1;
        InternalApplySnapshot(snapshot);
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public int Version { get; private set; }

    /// <summary>
    /// Applies the snapshot for rehydration.
    /// </summary>
    /// <param name="snapshot">The <see cref="TMemento"/> to apply.</param>
    protected abstract void ApplySnapshot(TMemento snapshot);

    private void InternalApplySnapshot(IMemento? snapshot)
    {
        if (snapshot is TMemento memento)
        {
            Version = snapshot.Version;
            ApplySnapshot(memento);
        }
    }

    /// <inheritdoc />
    void IAggregate.ApplyEvent(DomainEvent @event)
    {
        _registeredRoutes.Dispatch(@event);
        ++Version;
    }

    /// <inheritdoc />
    IEnumerable<DomainEvent> IAggregate.GetUncommittedEvents() => _uncommittedEvents;

    /// <inheritdoc />
    void IAggregate.ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }

    /// <inheritdoc />
    public abstract IMemento GetSnapshot();

    /// <inheritdoc />
    public virtual bool Equals(IAggregate? other) => other?.Id == Id && other.Version == Version;

    /// <summary>
    /// Applies the event and adds to the uncommitted event stream.
    /// </summary>
    /// <param name="event"></param>
    /// <typeparam name="T"></typeparam>
    protected void RaiseEvent<T>(T @event) where T : DomainEvent
    {
        ((IAggregate)this).ApplyEvent(@event);
        _uncommittedEvents.Add(@event);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as IAggregate);

    /// <inheritdoc />
    public virtual void Dispose()
    {
        _registeredRoutes.Dispose();
        _uncommittedEvents.Clear();
        GC.SuppressFinalize(this);
    }
}
