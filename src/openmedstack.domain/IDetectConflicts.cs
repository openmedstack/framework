using System.Collections.Generic;

namespace OpenMedStack.Domain
{
    public interface IDetectConflicts
    {
        void Register<TUncommitted, TCommitted>(ConflictDelegate<TUncommitted, TCommitted> handler) where TUncommitted : class where TCommitted : class;

        bool ConflictsWith(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents);
    }
}