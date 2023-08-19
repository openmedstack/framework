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
    private readonly IDispatchEvents _eventRouter;
    private readonly ICollection<BaseEvent> _uncommitted = new LinkedList<BaseEvent>();
    private readonly ICollection<DomainCommand> _undispatched = new LinkedList<DomainCommand>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaBase"/> class.
    /// </summary>
    /// <param name="id">The saga id.</param>
    /// <param name="eventRouter">The event router. If <c>null</c> is provided, then uses a <see cref="ConventionEventRouter"/>.</param>
    protected SagaBase(string id, IDispatchEvents? eventRouter = null)
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
    public virtual void Transition(BaseEvent message)
    {
        _eventRouter.Dispatch(message);

        _uncommitted.Add(message);
        ++Version;
    }

    /// <inheritdoc />
    IEnumerable<BaseEvent> ISaga.GetUncommittedEvents() => _uncommitted;

    /// <inheritdoc />
    void ISaga.ClearUncommittedEvents()
    {
        _uncommitted.Clear();
    }

    /// <inheritdoc />
    IEnumerable<DomainCommand> ISaga.GetUndispatchedMessages() => _undispatched;

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
