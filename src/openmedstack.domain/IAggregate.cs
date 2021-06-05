namespace OpenMedStack.Domain
{
    using System;
    using System.Collections.Generic;
    using OpenMedStack.Events;

    public interface IAggregate : IDisposable
    {
        string Id { get; }

        int Version { get; }

        void ApplyEvent(object @event);

        ICollection<DomainEvent> GetUncommittedEvents();

        void ClearUncommittedEvents();

        IMemento GetSnapshot();
    }
}