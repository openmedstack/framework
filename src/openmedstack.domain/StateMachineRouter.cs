namespace OpenMedStack.Domain;

using System;
using Microsoft.Extensions.Logging;
using Stateless;

public class StateMachineRouter<TState, TTrigger> : IDispatchEvents
{
    private readonly StateMachine<TState, TTrigger> _stateMachine;
    private readonly Func<object, TTrigger> _getTrigger;
    private readonly StateMachineTypeCache<TState, TTrigger> _typeCache;
    private readonly ILogger _logger;

    public StateMachineRouter(
        StateMachine<TState, TTrigger> stateMachine,
        Func<object, TTrigger> getTrigger,
        StateMachineTypeCache<TState, TTrigger> typeCache,
        ILogger logger)
    {
        _stateMachine = stateMachine;
        _getTrigger = getTrigger;
        _typeCache = typeCache;
        _logger = logger;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void Register<T>(Action<T> handler)
    {
    }

    public void Register(object aggregate)
    {
    }

    public void Dispatch(object eventMessage)
    {
        var trigger = _getTrigger(eventMessage);
        if (_stateMachine.CanFire(trigger))
        {
            try
            {
                var messageType = eventMessage.GetType();
                var fireMethod = _typeCache.GetFireMethod(messageType);
                var parameterInstance = _typeCache.GetParameterInstance(messageType, trigger);
                fireMethod.Invoke(_stateMachine, new[] { parameterInstance, eventMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{error}", ex.Message);
                throw;
            }
        }
        else
        {
            throw new InvalidTransitionException($"Invalid transition with {eventMessage.GetType().Name}");
        }
    }
}
