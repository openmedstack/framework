namespace OpenMedStack.Autofac.NEventStore.Sql.Tests;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack;
using OpenMedStack.Events;

internal class TestEventHandler : EventHandlerBase<TestEvent>
{
    /// <inheritdoc />
    public TestEventHandler(ILogger<EventHandlerBase<TestEvent>> logger) : base(logger)
    {
    }

    /// <inheritdoc />
    protected override Task HandleInternal(
        TestEvent domainEvent,
        IMessageHeaders headers,
        CancellationToken cancellationToken = new CancellationToken()) =>
        Task.CompletedTask;
}