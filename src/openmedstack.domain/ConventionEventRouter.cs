namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines the event router using a convention base on Apply methods in the source type.
/// </summary>
public sealed class ConventionEventRouter : EventRouterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConventionEventRouter"/> class.
    /// </summary>
    /// <param name="throwOnApplyNotFound"><c>true</c> when should throw when a handler is not found, otherwise <c>false</c>.</param>
    /// <param name="handlerSource">The source instance to search for handler methods.</param>
    public ConventionEventRouter(bool throwOnApplyNotFound, object handlerSource) : base(throwOnApplyNotFound)
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
