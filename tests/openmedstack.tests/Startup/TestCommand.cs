namespace OpenMedStack.Tests.Startup
{
    using System;
    using OpenMedStack.Commands;

    public record TestCommand : DomainCommand
    {
        /// <inheritdoc />
        public TestCommand(string aggregateId, int version, DateTimeOffset timeStamp, string? correlationId = null)
            : base(aggregateId, version, timeStamp, correlationId)
        {
        }
    }
}
