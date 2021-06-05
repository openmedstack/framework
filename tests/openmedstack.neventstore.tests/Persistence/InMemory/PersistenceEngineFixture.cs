// ReSharper disable once CheckNamespace
namespace OpenMedStack.NEventStore.Persistence.AcceptanceTests
{
    using OpenMedStack.NEventStore.Persistence.InMemory;

    public partial class PersistenceEngineFixture
    {
        public PersistenceEngineFixture()
        {
            _createPersistence = _ =>
                new InMemoryPersistenceEngine();
        }
    }
}