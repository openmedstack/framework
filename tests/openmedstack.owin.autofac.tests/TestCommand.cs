namespace OpenMedStack.Web.Autofac.Tests
{
    using System;
    using OpenMedStack.Commands;

    public class TestCommand : DomainCommand
    {
        /// <inheritdoc />
        public TestCommand()
            : base(Guid.NewGuid().ToString(), 0, DateTimeOffset.Now)
        {
        }
    }
}