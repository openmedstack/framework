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
using OpenMedStack.Commands;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;

/// <summary>
/// Defines the abstract base class for sagas.
/// </summary>
public abstract class SagaBase : ISaga, IEquatable<ISaga>
{
    private readonly IDispatchEvents _eventRouter;

    /// <summary>
    /// Initializes a new instance of the <see cref="SagaBase"/> class.
    /// </summary>
    /// <param name="id">The saga id.</param>
    /// <param name="stream">The event stream for the saga.</param>
    /// <param name="eventRouter">The event router. If <c>null</c> is provided, then uses a <see cref="ConventionEventRouter"/>.</param>
    protected SagaBase(string id, IEventStream stream, IDispatchEvents? eventRouter = null)
    {
        Stream = stream;
        _eventRouter = eventRouter ?? new ConventionEventRouter(true, this);
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
