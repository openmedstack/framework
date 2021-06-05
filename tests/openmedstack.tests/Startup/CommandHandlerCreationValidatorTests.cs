namespace OpenMedStack.Tests.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OpenMedStack.Commands;
    using OpenMedStack.Startup;
    using Xunit;

    public class CommandHandlerCreationValidatorTests
    {
        [Fact]
        public async Task WhenCommandHandlerCreationThrowsThenReturnsError()
        {
            var createFunc = new Func<IEnumerable<IHandleCommands>>(() => throw new Exception("test error"));

            var validator = new CommandHandlerCreationValidator(createFunc);
            var result = await validator.Validate().ConfigureAwait(false);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task WhenCommandHandlerCreationSucceedsThenDoesNotReturnError()
        {
            var createFunc = new Func<IEnumerable<IHandleCommands>>(() => new[] { new TestCommandHandler() });

            var validator = new CommandHandlerCreationValidator(createFunc);
            var result = await validator.Validate().ConfigureAwait(false);

            Assert.Null(result);
        }
    }
}