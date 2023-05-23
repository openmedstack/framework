namespace OpenMedStack.Web.Autofac.Tests;

using System;
using OpenMedStack.Commands;

[Topic("WebCommand")]
public record TestCommand : DomainCommand
{
    /// <inheritdoc />
    public TestCommand(string? correlationId = null)
        : base(Guid.NewGuid().ToString(), 0, DateTimeOffset.Now, correlationId)
    {
    }
}