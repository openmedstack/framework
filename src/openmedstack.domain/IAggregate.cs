namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using OpenMedStack.Events;

public interface IAggregate : IDisposable
{
    string Id { get; }

    int Version { get; }

    void ApplyEvent(DomainEvent @event);

    IEnumerable<DomainEvent> GetUncommittedEvents();

    void ClearUncommittedEvents();

    IMemento GetSnapshot();
}
