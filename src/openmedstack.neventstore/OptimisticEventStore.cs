using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMedStack.NEventStore.Logging;
using OpenMedStack.NEventStore.Persistence;

namespace OpenMedStack.NEventStore
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class OptimisticEventStore : IStoreEvents, ICommitEvents
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(OptimisticEventStore));
        private readonly IPersistStreams _persistence;
        private readonly IEnumerable<IPipelineHook> _pipelineHooks;

        public OptimisticEventStore(IPersistStreams persistence, IEnumerable<IPipelineHook> pipelineHooks)
        {
            if (persistence == null)
            {
                throw new ArgumentNullException(nameof(persistence));
            }

            _pipelineHooks = pipelineHooks ?? Array.Empty<IPipelineHook>();
            _persistence = new PipelineHooksAwarePersistanceDecorator(persistence, _pipelineHooks);
        }

        public virtual IAsyncEnumerable<ICommit> GetFrom(
            string bucketId,
            string streamId,
            int minRevision,
            int maxRevision,
            CancellationToken cancellationToken) => _persistence.GetFrom(bucketId, streamId, minRevision, maxRevision, cancellationToken);

        public virtual async Task<ICommit?> Commit(CommitAttempt attempt)
        {
            Guard.NotNull(nameof(attempt), attempt);
            foreach (var hook in _pipelineHooks)
            {
                Logger.Verbose(Resources.InvokingPreCommitHooks, attempt.CommitId, hook.GetType());
                if (await hook.PreCommit(attempt).ConfigureAwait(false))
                {
                    continue;
                }

                Logger.Info(Resources.CommitRejectedByPipelineHook, hook.GetType(), attempt.CommitId);
                return null;
            }

            Logger.Verbose(Resources.CommittingAttempt, attempt.CommitId, attempt.Events?.Count ?? 0);
            var commit = await _persistence.Commit(attempt).ConfigureAwait(false);

            if (commit != null)
            {
                foreach (var hook in _pipelineHooks)
                {
                    Logger.Verbose(Resources.InvokingPostCommitPipelineHooks, attempt.CommitId, hook.GetType());
                    await hook.PostCommit(commit).ConfigureAwait(false);
                }
            }

            return commit;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<IEventStream> CreateStream(string bucketId, string streamId)
        {
            Logger.Debug(Resources.CreatingStream, streamId, bucketId);
            return await OptimisticEventStream.Create(bucketId, streamId, this).ConfigureAwait(false);
        }

        public virtual async Task<IEventStream> OpenStream(
            string bucketId,
            string streamId,
            int minRevision,
            int maxRevision,
            CancellationToken cancellationToken)
        {
            if (streamId == null)
            {
                throw new ArgumentNullException();
            }
            maxRevision = maxRevision <= 0 ? int.MaxValue : maxRevision;

            Logger.Verbose(Resources.OpeningStreamAtRevision, streamId, bucketId, minRevision, maxRevision);
            return await OptimisticEventStream.Create(bucketId, streamId, this, minRevision, maxRevision, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IEventStream> OpenStream(ISnapshot snapshot, int maxRevision, CancellationToken cancellationToken)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            Logger.Verbose(Resources.OpeningStreamWithSnapshot, snapshot.StreamId, snapshot.StreamRevision, maxRevision);
            maxRevision = maxRevision <= 0 ? int.MaxValue : maxRevision;
            return await OptimisticEventStream.Create(snapshot, this, maxRevision, cancellationToken).ConfigureAwait(false);
        }

        public virtual IPersistStreams Advanced => _persistence;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Logger.Info(Resources.ShuttingDownStore);
            _persistence.Dispose();
            foreach (var hook in _pipelineHooks)
            {
                hook.Dispose();
            }
        }
    }
}