namespace OpenMedStack.Autofac.NEventStore.Sql.Tests
{
    using System;
    using OpenMedStack.Commands;

    internal record TestCommand : DomainCommand
    {
        /// <inheritdoc />
        public TestCommand(string aggregateId, int version)
            : base(aggregateId, version, DateTimeOffset.Now)
        {
        }
    }
}