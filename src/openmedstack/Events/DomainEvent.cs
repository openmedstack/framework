// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainEvent.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   The domain event base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Events
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    ///     The domain event base.
    /// </summary>
    public abstract class DomainEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// Create new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <param name="source">
        /// The id of the aggregate raising the event.
        /// </param>
        /// <param name="version">
        /// The version of the aggregate when the event was raised.
        /// </param>
        /// <param name="timeStamp">
        /// The time stamp the event was raised.
        /// </param>
        /// <param name="correlationId">
        /// The correlation id of the event.
        /// </param>
        protected DomainEvent(string source, int version, DateTimeOffset timeStamp, string? correlationId = null)
            : base(source, timeStamp, correlationId)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(source));
            Contract.Requires(version >= 0);
            Contract.Requires(timeStamp != DateTimeOffset.MinValue);

            Version = version;
        }

        /// <inheritdoc />
        public int Version { get; }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Source, Version, Timestamp, CorrelationId);
    }
}