namespace OpenMedStack.Domain;

using System.Collections.Generic;
using OpenMedStack.Events;

public interface ISaga
{
    string Id { get; }

    int Version { get; }

    void Transition(BaseEvent message);

    IEnumerable<object> GetUncommittedEvents();

    void ClearUncommittedEvents();

    IEnumerable<object> GetUndispatchedMessages();

    void ClearUndispatchedMessages();
}
