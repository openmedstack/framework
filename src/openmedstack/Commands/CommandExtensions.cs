// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the command extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Commands
{
    /// <summary>
    /// Defines the command extensions.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Creates a <see cref="CommandResponse"/> because entity does not exist.
        /// </summary>
        /// <param name="command">The <see cref="DomainCommand"/> to respond to.</param>
        /// <returns>An error <see cref="CommandResponse"/>.</returns>
        public static CommandResponse CreateEntityDoesNotExistResponse(this DomainCommand command) => new(
          command.AggregateId,
          command.Version,
          "Entity with id " + command.AggregateId + " does not exist",
          command.CorrelationId);

        /// <summary>
        /// Creates a <see cref="CommandResponse"/> because entity already exists.
        /// </summary>
        /// <param name="command">The <see cref="DomainCommand"/> to respond to.</param>
        /// <returns>An error <see cref="CommandResponse"/>.</returns>
        public static CommandResponse CreateEntityAlreadyExistResponse(this DomainCommand command) => new(
          command.AggregateId,
          command.Version,
          "Entity with id " + command.AggregateId + " already exist",
          command.CorrelationId);

        public static CommandResponse CreateResponse(this DomainCommand command, string? faultMessage = null) =>
          new(
            command.AggregateId,
            command.Version,
            faultMessage,
            command.CorrelationId);
    }
}