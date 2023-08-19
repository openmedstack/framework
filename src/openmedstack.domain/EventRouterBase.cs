namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;

public class EventRouterBase : IDispatchEvents
{
    private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
    private bool _throwOnApplyNotFound;

    protected EventRouterBase(bool throwOnApplyNotFound)
    {
        _throwOnApplyNotFound = throwOnApplyNotFound;
    }

    protected void Register(Type messageType, Action<object> handler)
    {
        _handlers[messageType] = handler;
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

    /// <inheritdoc />
    public void Dispose()
    {
        _handlers.Clear();
    }
}