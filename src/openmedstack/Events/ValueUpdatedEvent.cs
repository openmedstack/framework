// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueUpdatedEvent.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the abstract base class for value updated events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Events
{
    using System;

    /// <summary>
    /// Defines the abstract base class for value updated events.
    /// </summary>
    /// <typeparam name="TValue">The <see cref="Type"/> of the updated value.</typeparam>
    public abstract class ValueUpdatedEvent<TValue> : DomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueUpdatedEvent{TValue}"/> class.
        /// </summary>
        /// <param name="oldValue">The value before the update.</param>
        /// <param name="newValue">The value after the update.</param>
        /// <param name="aggregateId">The id of the aggregate raising the event.</param>
        /// <param name="version">The version of the aggregate when the event was raised.</param>
        /// <param name="timeStamp">The time stamp the event was raised.</param>
        /// <param name="correlationId">The correlation id of the event.</param>
        protected ValueUpdatedEvent(
            TValue oldValue,
            TValue newValue,
            string aggregateId,
            int version,
            DateTimeOffset timeStamp,
            string? correlationId = null)
            : base(aggregateId, version, timeStamp, correlationId)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the value before the update.
        /// </summary>
        public TValue OldValue { get; }

        /// <summary>
        /// Gets the value after the update.
        /// </summary>
        public TValue NewValue { get; }
    }
}
