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

    /// <summary>
    ///     The base command definition.
    /// </summary>
    public abstract record DomainCommand : Message, ICorrelate
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
        :base(timeStamp)
        {
            if (timeStamp == DateTimeOffset.MinValue)
            {
                throw new ArgumentException(Strings.CannotUseMinTime, nameof(timeStamp));
            }
            
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

        /// <inheritdoc />
        public string? CorrelationId { get; }
    }
}