using System;

namespace OpenMedStack.Domain;

/// <summary>
/// Defines the interface for dispatching events to the appropriate handler.
/// </summary>
public interface IDispatchEvents : IDisposable
{
    /// <summary>
    /// Dispatches the event message to the appropriate handler.
    /// </summary>
    /// <param name="eventMessage">The event message to dispatch.</param>
    void Dispatch(object eventMessage);
}
