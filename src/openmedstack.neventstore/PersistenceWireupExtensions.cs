using OpenMedStack.NEventStore.Logging;
using OpenMedStack.NEventStore.Persistence;
using OpenMedStack.NEventStore.Persistence.InMemory;

namespace OpenMedStack.NEventStore
{
    public static class PersistenceWireupExtensions
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(OptimisticPipelineHook));

        public static PersistenceWireup UsingInMemoryPersistence(this Wireup wireup)
        {
            Logger.Info(Resources.WireupSetPersistenceEngine, "InMemoryPersistenceEngine");
            wireup.With<IPersistStreams>(new InMemoryPersistenceEngine());

            return new PersistenceWireup(wireup);
        }

        public static int Records(this int records) => records;
    }
}