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

    /// <summary>
    ///     The domain event base.
    /// </summary>
    public abstract record DomainEvent : BaseEvent
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
            if (version <= 0)
            {
                throw new ArgumentException(Strings.VersionZeroInvalid, nameof(version));
            }

            Version = version;
        }

        /// <summary>
        /// Gets the event version.
        /// </summary>
        public int Version { get; }
    }
}