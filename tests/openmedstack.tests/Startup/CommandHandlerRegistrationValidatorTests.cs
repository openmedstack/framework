namespace OpenMedStack.Tests.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Commands;
    using OpenMedStack.Startup;
    using Moq;
    using Xunit;

    internal class FakeLogger<T> : ILogger<T>
    {
        public LogLevel LevelCalled { get; set; }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LevelCalled = logLevel;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }

    public class CommandHandlerRegistrationValidatorTests
    {
        [Fact]
        public async Task WhenServiceHasNoRegisteredCommandHandlerThenErrorIsLogged()
        {
            var configuration = new DeploymentConfiguration
            {
                Services = new Dictionary<Regex, Uri>
                {
                    {new Regex(".+"), new Uri("http://localhost")}
                }
            };

            var createFunc = new Func<IEnumerable<IHandleCommands>>(Enumerable.Empty<IHandleCommands>);
            var mock = new FakeLogger<CommandHandlerRegistrationValidator>();

            var validator = new CommandHandlerRegistrationValidator(
                configuration,
                createFunc,
                mock);

            await validator.Validate().ConfigureAwait(false);

            Assert.Equal(LogLevel.Warning, mock.LevelCalled);
        }

        [Fact]
        public async Task WhenServiceHasRegisteredCommandHandlerThenDoesNotReturnError()
        {
            var configuration = new DeploymentConfiguration
            {
                Services = new Dictionary<Regex, Uri>
                {
                    {new Regex(".+"), new Uri("http://localhost")}
                }
            };
            var createFunc = new Func<IEnumerable<IHandleCommands>>(() => new[] { new TestCommandHandler() });
            var validator = new CommandHandlerRegistrationValidator(
                configuration,
                createFunc,
                new Mock<ILogger<CommandHandlerRegistrationValidator>>().Object);

            var error = await validator.Validate().ConfigureAwait(false);

            Assert.Null(error);
        }
    }
}
