namespace OpenMedStack.Tests.Startup;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMedStack.Autofac.Startup;
using OpenMedStack.Commands;
using Xunit;

public class CommandHandlerCreationValidatorTests
{
    [Fact]
    public async Task WhenCommandHandlerCreationThrowsThenReturnsError()
    {
        var createFunc = new Func<IEnumerable<IHandleCommands>>(() => throw new Exception("test error"));

        var validator = new CommandHandlerCreationValidator(
            createFunc,
            new NullLogger<CommandHandlerCreationValidator>());
        var result = await validator.Validate().ConfigureAwait(false);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task WhenCommandHandlerCreationSucceedsThenDoesNotReturnError()
    {
        var createFunc = new Func<IEnumerable<IHandleCommands>>(() => new[] { new TestCommandHandler() });

        var validator = new CommandHandlerCreationValidator(createFunc, NullLogger<CommandHandlerCreationValidator>.Instance);
        var result = await validator.Validate().ConfigureAwait(false);

        Assert.Null(result);
    }
}
