namespace OpenMedStack.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Commands;

    public class CommandHandlerRegistrationValidator : IValidateStartup
    {
        private readonly DeploymentConfiguration _configuration;
        private readonly Func<IEnumerable<IHandleCommands>> _loaderFunc;
        private readonly ILogger<CommandHandlerRegistrationValidator> _logger;

        public CommandHandlerRegistrationValidator(
            DeploymentConfiguration configuration,
            Func<IEnumerable<IHandleCommands>> loaderFunc,
            ILogger<CommandHandlerRegistrationValidator> logger)
        {
            _configuration = configuration;
            _loaderFunc = loaderFunc;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<Exception?> Validate()
        {
            IEnumerable<IHandleCommands> handlers;
            try
            {
                handlers = _loaderFunc();
            }
            catch
            {
                return Task.FromResult<Exception?>(null);
            }

            var commandTypes = (from handler in handlers
                               from commandType in handler.GetType()
                                   .GetInterfaces()
                                   .Where(t => t.IsGenericType)
                                   .Select(t => t.GetGenericArguments())
                                   .Where(x => x.Length == 1)
                                   .Select(x => x[0])
                                   .Where(t => typeof(DomainCommand).IsAssignableFrom(t))
                               select commandType).ToArray();
            var errors = (from configurationService in _configuration.Services
                          where !commandTypes.Any(t => configurationService.Key.IsMatch(t.FullName!))
                          select "No commands target " + configurationService.Value.AbsoluteUri).ToArray();

            if (errors.Length <= 0)
            {
                return Task.FromResult<Exception?>(null);
            }

            var aggregateException = new AggregateException(
                "Failed to validate command handler registration",
                errors.Select(x => new Exception(x)));
            foreach (var error in aggregateException.InnerExceptions)
            {
                _logger.LogError(error, "{error}", error.Message);
            }

            _logger.LogInformation("Validated {count} command handlers", commandTypes.Length);
            return Task.FromResult<Exception?>(aggregateException);
        }
    }
}
