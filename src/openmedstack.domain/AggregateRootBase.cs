// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateRootBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the AggregateRootBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain
{
    using System;
    using System.Collections.Generic;
    using OpenMedStack.Events;

    public abstract class AggregateRootBase<TMemento> : IAggregate, IEquatable<IAggregate> where TMemento : class, IMemento
    {
        private readonly ICollection<DomainEvent> _uncommittedEvents = new LinkedList<DomainEvent>();
        private readonly IRouteEvents _registeredRoutes;

        protected AggregateRootBase(string id, IMemento? snapshot)
        {
            _registeredRoutes = new ConventionEventRouter(true, this);
            Id = id;
            InternalApplySnapshot(snapshot);
        }

        public string Id { get; }

        public int Version { get; private set; }

        protected abstract void ApplySnapshot(TMemento snapshot);

        private void InternalApplySnapshot(IMemento? snapshot)
        {
            if (snapshot is TMemento memento)
            {
                Version = snapshot.Version;
                ApplySnapshot(memento);
            }
        }

        void IAggregate.ApplyEvent(object @event)
        {
            _registeredRoutes.Dispatch(@event);
            ++Version;
        }

        ICollection<DomainEvent> IAggregate.GetUncommittedEvents() => _uncommittedEvents;

        void IAggregate.ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        IMemento IAggregate.GetSnapshot()
        {
            var snapshot = CreateSnapshot(Id, Version);
            return snapshot;
        }

        public virtual bool Equals(IAggregate? other) => other?.Id == Id && other.Version == Version;

        protected void RaiseEvent<T>(T @event) where T : DomainEvent
        {
            ((IAggregate)this).ApplyEvent(@event);
            _uncommittedEvents.Add(@event);
        }

        protected abstract TMemento CreateSnapshot(string id, int version);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object? obj) => Equals(obj as IAggregate);

        public virtual void Dispose()
        {
            _registeredRoutes.Dispose();
            _uncommittedEvents.Clear();
            GC.SuppressFinalize(this);
        }
    }
}