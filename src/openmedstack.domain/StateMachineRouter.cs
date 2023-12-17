namespace OpenMedStack.Domain;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using OpenMedStack.Events;
using Stateless;

/// <summary>
/// Defines the state machine implementation of <see cref="IDispatchEvents"/>.
/// </summary>
/// <typeparam name="TState">The <see cref="Type"/> of states.</typeparam>
/// <typeparam name="TTrigger">The <see cref="Type"/> of triggers.</typeparam>
public class StateMachineRouter<
    TState, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TTrigger> : IDispatchEvents
{
    private readonly StateMachine<TState, TTrigger> _stateMachine;
    private readonly Func<object, TTrigger> _getTrigger;
    private readonly StateMachineTypeCache<TState, TTrigger> _typeCache;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateMachineRouter{TState,TTrigger}"/> class.
    /// </summary>
    /// <param name="stateMachine">The underlying process state machine.</param>
    /// <param name="getTrigger">The <see cref="Func{TInput,TResult}"/> to get the trigger.</param>
    /// <param name="typeCache">The <see cref="StateMachineTypeCache{TState,TTrigger}"/> for reflection performance.</param>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/>.</param>
    public StateMachineRouter(
        StateMachine<TState, TTrigger> stateMachine,
        Func<object, TTrigger> getTrigger,
        StateMachineTypeCache<TState, TTrigger> typeCache,
        ILogger<StateMachineRouter<TState, TTrigger>> logger)
    {
        _stateMachine = stateMachine;
        _getTrigger = getTrigger;
        _typeCache = typeCache;
        _logger = logger;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
        Justification = "<Pending>")]
    public void Dispatch(BaseEvent eventMessage)
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
                _logger.LogError(ex, "{Error}", ex.Message);
                throw;
            }
        }
        else
        {
            throw new InvalidTransitionException($"Invalid transition with {eventMessage.GetType().Name}");
        }
    }
}
