namespace OpenMedStack.Domain;

using System;
using Microsoft.Extensions.Logging;
using OpenMedStack.Commands;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;
using Stateless;

/// <summary>
/// Defines the abstract base class for sagas.
/// </summary>
public abstract class StateMachineSagaBase<TState, TTrigger> : ISaga, IEquatable<ISaga>
{
    private readonly ILogger _logger;
    private readonly StateMachineRouter<TState, TTrigger> _eventRouter;

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaBase"/> class.
    /// </summary>
    /// <param name="id">The saga id.</param>
    /// <param name="stream">The event stream for the saga.</param>
    /// <param name="initialState">The initial state of the state machine</param>
    /// <param name="typeCache">The <see cref="StateMachineTypeCache{TState,TTrigger}"/> to improve reflection performance.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    protected StateMachineSagaBase(
        string id,
        IEventStream stream,
        TState initialState,
        StateMachineTypeCache<TState, TTrigger> typeCache,
        ILoggerFactory loggerFactory)
    {
        Stream = stream;
        _logger = loggerFactory.CreateLogger(GetType());
        var stateMachine = new StateMachine<TState, TTrigger>(initialState);
        stateMachine.OnTransitionCompleted(
            transition => _logger.LogInformation(
                "Transitioned from {Source} to {Destination} with trigger {Trigger}",
                transition.Source,
                transition.Destination,
                transition.Trigger));
        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureStateMachine(stateMachine);
        _eventRouter = new StateMachineRouter<TState, TTrigger>(
            stateMachine,
            GetTrigger,
            typeCache,
            loggerFactory.CreateLogger<StateMachineRouter<TState, TTrigger>>());
        Id = id;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public int Version { get; private set; }

    /// <inheritdoc />
    public IEventStream Stream { get; }

    /// <inheritdoc />
    public virtual bool Equals(ISaga? other) => other?.Id == Id;

    /// <inheritdoc />
    public virtual void Transition(BaseEvent message)
    {
        _eventRouter.Dispatch(message);
        Stream.Add(new EventMessage(message));
        ++Version;
    }

    protected abstract void ConfigureStateMachine(StateMachine<TState, TTrigger> stateMachine);

    protected abstract TTrigger GetTrigger(object message);

    /// <summary>
    /// Dispatches the <see cref="DomainCommand"/>.
    /// </summary>
    /// <param name="message"></param>
    protected void Dispatch(DomainCommand message)
    {
        Stream.Add($"UndispatchedMessage.{Stream.UncommittedHeaders.Count}", message);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ISaga);
}
