namespace OpenMedStack.Domain.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using OpenMedStack.ReadModels;

    internal class TestDataStoreUpdater : IUpdateReadModel<TestEvent>
    {
        private readonly TestDataStore _dataStore;

        public TestDataStoreUpdater(TestDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <inheritdoc />
        public Task Update(
            TestEvent domainEvent,
            IMessageHeaders headers,
            CancellationToken cancellationToken = default)
        {
            _dataStore.Updates++;
            return Task.CompletedTask;
        }
    }
}