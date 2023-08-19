namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using OpenMedStack.Events;

/// <summary>
/// Defines the abstract event router.
/// </summary>
public abstract class EventRouterBase : IDispatchEvents
{
    private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
    private readonly bool _throwOnApplyNotFound;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventRouterBase"/> class.
    /// </summary>
    /// <param name="throwOnApplyNotFound"><c>true</c> when should throw when a handler is not found, otherwise <c>false</c>.</param>
    protected EventRouterBase(bool throwOnApplyNotFound)
    {
        _throwOnApplyNotFound = throwOnApplyNotFound;
    }

    /// <summary>
    /// Registers a handler for a message type.
    /// </summary>
    /// <param name="messageType">The message <see cref="Type"/>.</param>
    /// <param name="handler">The handler.</param>
    protected void Register(Type messageType, Action<object> handler)
    {
        _handlers[messageType] = handler;
    }

    /// <inheritdoc />
    public void Dispatch(BaseEvent eventMessage)
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
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _handlers.Clear();
    }
}
