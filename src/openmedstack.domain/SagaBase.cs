// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SagaBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the abstract base class for sagas.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain
{
    using System;
    using System.Collections.Generic;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;

    /// <summary>
    /// Defines the abstract base class for sagas.
    /// </summary>
    /// <typeparam name="TEvent">The type constraint for <see cref="BaseEvent"/> which can be handled.</typeparam>
    /// <typeparam name="TCommand">The type constraint for <see cref="DomainCommand"/> which will be sent.</typeparam>
    public abstract class SagaBase<TEvent, TCommand> : ISaga, IEquatable<ISaga>
       where TEvent : BaseEvent
       where TCommand : DomainCommand
    {
        private readonly IRouteEvents _eventRouter;
        private readonly ICollection<TEvent> _uncommitted = new LinkedList<TEvent>();
        private readonly ICollection<TCommand> _undispatched = new LinkedList<TCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SagaBase{TEvent,TCommand}"/> class.
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
        public void Transition(object message)
        {
            if (message is not TEvent item)
            {
                return;
            }

            _eventRouter.Dispatch(item);

            _uncommitted.Add(item);
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
           where TRegisteredMessage : class, TEvent
        {
            _eventRouter.Register(handler);
        }

        /// <summary>
        /// Dispatches the <see cref="TCommand"/>.
        /// </summary>
        /// <param name="message"></param>
        protected void Dispatch(TCommand message)
        {
            _undispatched.Add(message);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Id.GetHashCode();

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as ISaga);
    }
}