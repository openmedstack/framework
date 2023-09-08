namespace OpenMedStack.Autofac.Startup;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMedStack.Events;
using OpenMedStack.Startup;

public class EventHandlerCreationValidator : IValidateStartup
{
    private readonly Func<IEnumerable<IHandleEvents>> _loaderFunc;
    private readonly ILogger<EventHandlerCreationValidator> _logger;

    public EventHandlerCreationValidator(
        Func<IEnumerable<IHandleEvents>> loaderFunc,
        ILogger<EventHandlerCreationValidator> logger)
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
            _logger.LogInformation("Validated {count} event handlers", handlers.Length);
            return Task.FromResult<Exception?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{error}", ex.Message);
            return Task.FromResult<Exception?>(ex);
        }
    }
}
