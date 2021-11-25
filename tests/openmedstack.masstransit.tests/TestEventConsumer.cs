namespace openmedstack.masstransit.tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;

    internal class TestEventConsumer : IConsumer<TestEvent>
    {
        private readonly ManualResetEventSlim _waitHandle;

        public TestEventConsumer(ManualResetEventSlim waitHandle)
        {
            _waitHandle = waitHandle;
        }

        /// <inheritdoc />
        public Task Consume(ConsumeContext<TestEvent> context)
        {
            _waitHandle.Set();
            return Task.CompletedTask;
        }
    }
}