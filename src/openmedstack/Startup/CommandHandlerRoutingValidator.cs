namespace OpenMedStack.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OpenMedStack.Commands;

    public class CommandHandlerRoutingValidator : IValidateStartup
    {
        private readonly DeploymentConfiguration _configuration;
        private readonly Func<IEnumerable<IHandleCommands>> _loaderFunc;

        public CommandHandlerRoutingValidator(DeploymentConfiguration configuration, Func<IEnumerable<IHandleCommands>> loaderFunc)
        {
            _configuration = configuration;
            _loaderFunc = loaderFunc;
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
            var errors = (from handler in handlers
                          from commandType in handler.GetType()
                              .GetInterfaces()
                              .Where(t => t.IsGenericType)
                              .Select(t => t.GetGenericArguments())
                              .Where(x => x.Length == 1)
                              .Select(x => x[0])
                              .Where(t => typeof(DomainCommand).IsAssignableFrom(t))
                          where !_configuration.Services.Any(x => x.Key.IsMatch(commandType.FullName!))
                          select new Exception($"{commandType.FullName} is not handled by any registered endpoint."))
                .ToList();

            return errors.Count switch
            {
                0 => Task.FromResult<Exception?>(null),
                1 => Task.FromResult<Exception?>(errors[0]),
                _ => Task.FromResult<Exception?>(new AggregateException(errors)),
            };
        }
    }
}