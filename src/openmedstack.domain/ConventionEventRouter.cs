namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ConventionEventRouter : EventRouterBase
{
    public ConventionEventRouter(bool throwOnApplyNotFound, object handlerSource) : base(throwOnApplyNotFound)
    {
        Register(handlerSource);
    }

    public void Register(object handlerSource)
    {
        if (handlerSource is null)
        {
            throw new ArgumentNullException(nameof(handlerSource));
        }

        var handlers = from m in HandlerCache.Instance.GetHandlers(handlerSource.GetType())
                       select new KeyValuePair<Type, Action<object>>(
                           m.messageType,
                           x => m.method.Invoke(handlerSource, new[] { x }));
        foreach (var (key, value) in handlers)
        {
            Register(key, value);
        }
    }
}
