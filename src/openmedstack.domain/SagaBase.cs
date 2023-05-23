// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the abstract base class for sagas.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain;

using System;
using System.Collections.Generic;
using OpenMedStack.Commands;
using OpenMedStack.Events;

/// <summary>
/// Defines the abstract base class for sagas.
/// </summary>
public abstract class SagaBase : ISaga, IEquatable<ISaga>
{
    private readonly IRouteEvents _eventRouter;
    private readonly ICollection<BaseEvent> _uncommitted = new LinkedList<BaseEvent>();
    private readonly ICollection<DomainCommand> _undispatched = new LinkedList<DomainCommand>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaBase"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eventRouter">The event router</param>
    protected SagaBase(string id, IRouteEvents? eventRouter = null)
    {
        _eventRouter = eventRouter ?? new ConventionEventRouter(true, this);
        Id = id;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public int Version { get; private set; }

    /// <inheritdoc />
    public virtual bool Equals(ISaga? other) => other?.Id == Id;

    /// <inheritdoc />
    public void Transition(DomainEvent message)
    {
        _eventRouter.Dispatch(message);

        _uncommitted.Add(message);
        ++Version;
    }

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
    /// Registers the event handler.
    /// </summary>
    /// <typeparam name="TRegisteredMessage"></typeparam>
    /// <param name="handler"></param>
    protected void Register<TRegisteredMessage>(Action<TRegisteredMessage> handler)
        where TRegisteredMessage : DomainEvent
    {
        _eventRouter.Register(handler);
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
