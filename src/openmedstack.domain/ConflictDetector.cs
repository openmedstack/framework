using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMedStack.Domain
{
    /// <summary>
    ///   The conflict detector is used to determine if the events to be committed represent
    ///   a true business conflict as compared to events that have already been committed, thus
    ///   allowing reconciliation of optimistic concurrency problems.
    /// </summary>
    /// <remarks>
    ///   The implementation contains some internal lambda "magic" which allows casting between
    ///   TCommitted, TUncommitted, and System.Object and in a completely type-safe way.
    /// </remarks>
    public class ConflictDetector : IDetectConflicts
    {
        private readonly IDictionary<Type, IDictionary<Type, ConflictPredicate>> _actions = new Dictionary<Type, IDictionary<Type, ConflictPredicate>>();

        public void Register<TUncommitted, TCommitted>(ConflictDelegate<TUncommitted, TCommitted> handler) where TUncommitted : class where TCommitted : class
        {
            if (!_actions.TryGetValue(typeof(TUncommitted), out var dictionary))
            {
                _actions[typeof(TUncommitted)] = dictionary = new Dictionary<Type, ConflictPredicate>();
            }
            dictionary[typeof(TCommitted)] = (uncommitted, committed) => handler((uncommitted as TUncommitted)!, (committed as TCommitted)!);
        }

        public bool ConflictsWith(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents)
        {
            return uncommittedEvents.SelectMany(_ => committedEvents, (uncommitted, committed) => new
            {
                uncommitted,
                committed
            }).Where(param1 => Conflicts(param1.uncommitted, param1.committed)).Select(_ => uncommittedEvents).Any();
        }

        private bool Conflicts(object uncommitted, object committed)
        {
            if (!_actions.TryGetValue(uncommitted.GetType(), out var dictionary))
            {
                return uncommitted.GetType() == committed.GetType();
            }
            if (!dictionary.TryGetValue(committed.GetType(), out var conflictPredicate))
            {
                return true;
            }
            return conflictPredicate(uncommitted, committed);
        }

        private delegate bool ConflictPredicate(object uncommitted, object committed);
    }
}