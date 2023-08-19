namespace OpenMedStack.Web.Autofac.Tests;

using System;
using OpenMedStack.Events;

[Topic("WebTestDomainEvent")]
public record TestDomainEvent : DomainEvent
{
    /// <inheritdoc />
    public TestDomainEvent(string source, int version) : base(source, version, DateTimeOffset.Now)
    {
    }
}
