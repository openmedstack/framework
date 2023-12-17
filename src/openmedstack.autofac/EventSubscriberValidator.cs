namespace OpenMedStack.Autofac;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using global::Autofac;
using OpenMedStack.Events;
using OpenMedStack.Startup;

internal class EventSubscriberValidator : IValidateStartup
{
    private readonly ILifetimeScope _container;

    public EventSubscriberValidator(ILifetimeScope container)
    {
        _container = container;
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    public Task<Exception?> Validate()
    {
        var exceptions = new List<Exception>();
        var eventHandlers = _container.Resolve<IEnumerable<IHandleEvents>>();
        var eventTypes = from handler in eventHandlers
                         from interfaceType in handler.GetType().GetInterfaces()
                         where interfaceType.IsGenericType
                          && typeof(IHandleEvents<>).IsAssignableFrom(interfaceType.GetGenericTypeDefinition())
                         let eventType = interfaceType.GetGenericArguments()[0]
                         where typeof(BaseEvent).IsAssignableFrom(eventType)
                         select eventType;
        foreach (var eventType in eventTypes)
        {
            try
            {
                _ = _container.Resolve(typeof(ISubscribeEvents<>).MakeGenericType(eventType));
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }
        }

        return exceptions.Count switch
        {
            0 => Task.FromResult<Exception?>(null),
            1 => Task.FromResult<Exception?>(exceptions[0]),
            _ => Task.FromResult<Exception?>(new AggregateException(exceptions))
        };
    }
}
