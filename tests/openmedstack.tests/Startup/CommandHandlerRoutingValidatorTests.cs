namespace OpenMedStack.Tests.Startup;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMedStack.Autofac.Startup;
using OpenMedStack.Commands;
using Xunit;

public class CommandHandlerRoutingValidatorTests
{
    [Fact]
    public async Task WhenCommandHandlerHasNoTargetEndpointThenReturnsError()
    {
        var configuration = new DeploymentConfiguration();
        var createFunc = new Func<IEnumerable<IHandleCommands>>(() => new[] { new TestCommandHandler() });
        var validator = new CommandHandlerRoutingValidator(
            configuration,
            createFunc,
            NullLogger<CommandHandlerRoutingValidator>.Instance);

        var error = await validator.Validate();

        Assert.NotNull(error);
    }

    [Fact]
    public async Task WhenCommandHandlerHasTargetEndpointThenDoesNotReturnError()
    {
        var configuration = new DeploymentConfiguration
        {
            Services = new Dictionary<Regex, Uri>
            {
                { new Regex(".+"), new Uri("http://localhost") }
            }
        };
        var createFunc = new Func<IEnumerable<IHandleCommands>>(() => new[] { new TestCommandHandler() });
        var validator = new CommandHandlerRoutingValidator(
            configuration,
            createFunc,
            NullLogger<CommandHandlerRoutingValidator>.Instance);

        var error = await validator.Validate();

        Assert.Null(error);
    }
}
