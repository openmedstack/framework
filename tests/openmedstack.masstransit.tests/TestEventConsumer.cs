namespace openmedstack.masstransit.tests;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack;
using OpenMedStack.Events;

internal class TestEventConsumer : EventHandlerBase<TestEvent>
{
    private readonly ManualResetEventSlim _waitHandle;

    public TestEventConsumer(ManualResetEventSlim waitHandle, ILogger<TestEventConsumer> logger) : base(logger)
    {
        _waitHandle = waitHandle;
    }

    /// <inheritdoc />
    protected override Task HandleInternal(
        TestEvent domainEvent,
        IMessageHeaders headers,
        CancellationToken cancellationToken = default)
    {
        _waitHandle.Set();
        return Task.CompletedTask;
    }
}
