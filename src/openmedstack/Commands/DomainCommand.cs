// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainCommand.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   The base command definition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Commands
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    ///     The base command definition.
    /// </summary>
    public abstract class DomainCommand : ICorrelate, IEquatable<DomainCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainCommand"/> class.
        /// Create an instance of the <see cref="DomainCommand"/> class.
        /// </summary>
        /// <param name="aggregateId">
        /// The id of the aggregate the command targets.
        /// </param>
        /// <param name="version">
        /// The version of the aggregate the command targets.
        /// </param>
        /// <param name="timeStamp">
        /// The time stamp of the command.
        /// </param>
        /// <param name="correlationId">
        /// The correlation id of the command.
        /// </param>
        protected DomainCommand(string aggregateId, int version, DateTimeOffset timeStamp, string? correlationId = null)
        {
            if (timeStamp == DateTimeOffset.MinValue)
            {
                throw new ArgumentException("Cannot use min time", nameof(timeStamp));
            }

            Timestamp = timeStamp;
            AggregateId = aggregateId;
            Version = version;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Gets the id of the aggregate to target.
        /// </summary>
        public string AggregateId { get; }

        /// <summary>
        /// Gets the version the command targets.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Gets the time stamp of the command.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <inheritdoc />
        public string? CorrelationId { get; }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(AggregateId, Version, Timestamp, CorrelationId);

        /// <inheritdoc />
        public bool Equals(DomainCommand? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.GetType() == GetType()
                   && AggregateId == other.AggregateId
                   && Version == other.Version
                   && Timestamp.Equals(other.Timestamp)
                   && CorrelationId == other.CorrelationId;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is DomainCommand other && Equals(other);

        public static bool operator ==(DomainCommand left, DomainCommand right) => Equals(left, right);

        public static bool operator !=(DomainCommand left, DomainCommand right) => !Equals(left, right);
    }
}