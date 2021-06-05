namespace OpenMedStack.NEventStore.Persistence.InMemory
{
    public class InMemoryPersistenceFactory : IPersistenceFactory
    {
        public virtual IPersistStreams Build() => new InMemoryPersistenceEngine();
    }
}