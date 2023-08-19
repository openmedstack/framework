namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OpenMedStack.Commands;
using OpenMedStack.Events;
using Stateless;

/// <summary>
/// Defines the abstract base class for sagas.
/// </summary>
public abstract class StateMachineSagaBase<TState, TTrigger> : ISaga, IEquatable<ISaga>
{
    private readonly StateMachineRouter<TState, TTrigger> _eventRouter;
    private readonly ICollection<BaseEvent> _uncommitted = new LinkedList<BaseEvent>();
    private readonly ICollection<DomainCommand> _undispatched = new LinkedList<DomainCommand>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaBase"/> class.
    /// </summary>
    /// <param name="id">The saga id.</param>
    /// <param name="initialState">The initial state of the state machine</param>
    /// <param name="typeCache">The <see cref="StateMachineTypeCache{TState,TTrigger}"/> to improve reflection performance.</param>
    /// <param name="logger">The saga logger</param>
    protected StateMachineSagaBase(
        string id,
        TState initialState,
        StateMachineTypeCache<TState, TTrigger> typeCache,
        ILogger<StateMachineSagaBase<TState, TTrigger>> logger)
    {
        var stateMachine = new StateMachine<TState, TTrigger>(initialState);
        stateMachine.OnTransitionCompleted(
            transition => logger.LogInformation(
                "Transitioned from {source} to {destination} with trigger {trigger}",
                transition.Source,
                transition.Destination,
                transition.Trigger));
        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureStateMachine(stateMachine);
        _eventRouter = new StateMachineRouter<TState, TTrigger>(stateMachine, GetTrigger, typeCache, logger);
        Id = id;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public int Version { get; private set; }

    /// <inheritdoc />
    public virtual bool Equals(ISaga? other) => other?.Id == Id;

    /// <inheritdoc />
    public virtual void Transition(BaseEvent message)
    {
        _eventRouter.Dispatch(message);

        _uncommitted.Add(message);
        ++Version;
    }

    protected abstract void ConfigureStateMachine(StateMachine<TState, TTrigger> stateMachine);

    protected abstract TTrigger GetTrigger(object message);

    IEnumerable<object> ISaga.GetUncommittedEvents() => _uncommitted;

    void ISaga.ClearUncommittedEvents()
    {
        _uncommitted.Clear();
    }

    IEnumerable<object> ISaga.GetUndispatchedMessages() => _undispatched;

    void ISaga.ClearUndispatchedMessages()
    {
        _undispatched.Clear();
    }

    /// <summary>
    /// Dispatches the <see cref="DomainCommand"/>.
    /// </summary>
    /// <param name="message"></param>
    protected void Dispatch(DomainCommand message)
    {
        _undispatched.Add(message);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ISaga);
}
