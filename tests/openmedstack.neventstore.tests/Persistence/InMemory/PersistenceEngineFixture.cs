namespace OpenMedStack.NEventStore.Tests.Persistence.InMemory
{
    using OpenMedStack.NEventStore.Persistence.AcceptanceTests;
    using OpenMedStack.NEventStore.Persistence.InMemory;

    public class PersistenceEngineFixture : PersistenceEngineFixtureBase
    {
        public PersistenceEngineFixture()
        {
            CreatePersistence = _ =>
                new InMemoryPersistenceEngine();
        }
    }
}