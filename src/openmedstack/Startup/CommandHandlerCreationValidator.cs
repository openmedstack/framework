namespace OpenMedStack.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Commands;

    public class CommandHandlerCreationValidator : IValidateStartup
    {
        private readonly Func<IEnumerable<IHandleCommands>> _loaderFunc;
        private readonly ILogger<CommandHandlerCreationValidator> _logger;

        public CommandHandlerCreationValidator(
            Func<IEnumerable<IHandleCommands>> loaderFunc,
            ILogger<CommandHandlerCreationValidator> logger)
        {
            _loaderFunc = loaderFunc;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<Exception?> Validate()
        {
            try
            {
                var handlers = _loaderFunc().ToArray();
                _logger.LogInformation("Validated {count} command handlers", handlers.Length);
                return Task.FromResult<Exception?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate command handlers");
                return Task.FromResult<Exception?>(ex);
            }
        }
    }
}
