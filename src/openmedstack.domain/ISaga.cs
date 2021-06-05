namespace OpenMedStack.Domain
{
    using System.Collections.Generic;

    public interface ISaga
    {
        string Id { get; }

        int Version { get; }

        void Transition(object message);

        IEnumerable<object> GetUncommittedEvents();

        void ClearUncommittedEvents();

        IEnumerable<object> GetUndispatchedMessages();

        void ClearUndispatchedMessages();
    }
}