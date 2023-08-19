namespace OpenMedStack.Domain;

using System;

public sealed class DelegateEventRouter : EventRouterBase
{
    /// <inheritdoc />
    public DelegateEventRouter(bool throwOnApplyNotFound) : base(throwOnApplyNotFound)
    {
    }

    public void Register<T>(Action<T> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        Register(typeof(T), @event => handler((T)@event));
    }
}