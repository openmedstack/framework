namespace OpenMedStack.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OpenMedStack.Commands;

    public class CommandHandlerCreationValidator : IValidateStartup
    {
        private readonly Func<IEnumerable<IHandleCommands>> _loaderFunc;

        public CommandHandlerCreationValidator(Func<IEnumerable<IHandleCommands>> loaderFunc)
        {
            _loaderFunc = loaderFunc;
        }

        /// <inheritdoc />
        public Task<Exception?> Validate()
        {
            try
            {
                var handlers = _loaderFunc().ToArray();
                return Task.FromResult<Exception?>(null);
            }
            catch (Exception ex)
            {
                return Task.FromResult<Exception?>(ex);
            }
        }
    }
}