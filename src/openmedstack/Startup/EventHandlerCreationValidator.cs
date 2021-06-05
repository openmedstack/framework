namespace OpenMedStack.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OpenMedStack.Events;

    public class EventHandlerCreationValidator : IValidateStartup
    {
        private readonly Func<IEnumerable<IHandleEvents>> _loaderFunc;

        public EventHandlerCreationValidator(Func<IEnumerable<IHandleEvents>> loaderFunc)
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