namespace OpenMedStack.Domain;

using System;
using OpenMedStack.Events;

/// <summary>
/// Defines the event router using distinct delegates.
/// </summary>
public sealed class DelegateEventRouter : EventRouterBase
{
    /// <inheritdoc />
    public DelegateEventRouter(bool throwOnApplyNotFound) : base(throwOnApplyNotFound)
    {
    }

    /// <summary>
    /// Registers the concrete event handler.
    /// </summary>
    /// <param name="handler">The <see cref="Action{T}"/> to register.</param>
    /// <typeparam name="T">The <see cref="Type"/> of event to handle.</typeparam>
    public void Register<T>(Action<T> handler) where T : BaseEvent
    {
        Register(typeof(T), @event => handler((T)@event));
    }
}
