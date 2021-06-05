namespace OpenMedStack.Tests.Startup
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenMedStack.Events;

    public class TestEventHandler : IHandleEvents<TestEvent>
    {
        /// <inheritdoc />
        public Task Handle(TestEvent domainEvent, IMessageHeaders headers, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public bool CanHandle(Type type) => throw new NotImplementedException();
    }
}