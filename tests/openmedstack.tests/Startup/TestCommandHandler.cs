namespace OpenMedStack.Tests.Startup;

using System;
using System.Threading;
using System.Threading.Tasks;
using OpenMedStack.Commands;

public class TestCommandHandler : IHandleCommands<TestCommand>
{
    /// <inheritdoc />
    public Task<CommandResponse> Handle(
        TestCommand command,
        IMessageHeaders headers,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
