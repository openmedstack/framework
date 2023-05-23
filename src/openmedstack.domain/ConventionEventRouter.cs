namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ConventionEventRouter : IRouteEvents
{
    private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
    private readonly bool _throwOnApplyNotFound;

    public ConventionEventRouter(bool throwOnApplyNotFound, object handlerSource)
    {
        _throwOnApplyNotFound = throwOnApplyNotFound;
        Register(handlerSource);
    }

    public void Register<T>(Action<T> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        Register(typeof(T), @event => handler((T) @event));
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
                           x => m.method.Invoke(handlerSource, new[] {x}));
        foreach (var handler in handlers)
        {
            _handlers.Add(handler);
        }
    }

    public void Dispatch(object eventMessage)
    {
        if (eventMessage == null)
        {
            throw new ArgumentNullException(nameof(eventMessage));
        }

        if (_handlers.TryGetValue(eventMessage.GetType(), out var action))
        {
            action(eventMessage);
        }
        else
        {
            if (!_throwOnApplyNotFound)
            {
                return;
            }

            throw new HandlerForDomainEventNotFoundException(
                $"Aggregate of type '{GetType().Name}' raised an event of type '{eventMessage.GetType().Name}' but not handler could be found to handle the message.");
        }
    }

    private void Register(Type messageType, Action<object> handler)
    {
        _handlers[messageType] = handler;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _handlers.Clear();
    }
}
