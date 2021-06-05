namespace OpenMedStack.Autofac.NEventstore
{
    using System.Threading.Tasks;
    using OpenMedStack.Domain;

    public abstract class InMemoryCheckpointTracker : ITrackCheckpoints
    {
        private long _latest;

        /// <inheritdoc />
        public Task<long> GetLatest() => Task.FromResult(_latest);

        public Task SetLatest(long value)
        {
            _latest = value;
            return Task.CompletedTask;
        }
    }

    public class InMemoryEventCheckpointTracker : InMemoryCheckpointTracker, ITrackEventCheckpoints
    {
    }

    public class InMemoryCommandCheckpointTracker : InMemoryCheckpointTracker, ITrackCommandCheckpoints
    {
    }

    public class InMemoryReadModelCheckpointTracker : InMemoryCheckpointTracker, ITrackReadModelCheckpoints
    {
    }
}
