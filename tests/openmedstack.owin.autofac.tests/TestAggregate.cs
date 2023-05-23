namespace OpenMedStack.Web.Autofac.Tests;

using System.Diagnostics;
using OpenMedStack.Domain;

internal class TestAggregate : DefaultAggregateRoot
{
    /// <inheritdoc />
    public TestAggregate(string id, IMemento snapshot) : base(id, snapshot)
    {
    }

    public void DoSomething()
    {
        var evt = new TestDomainEvent(Id, Version);
        RaiseEvent(evt);
    }

    private void Apply(TestDomainEvent evt)
    {
        Trace.TraceInformation("Event applied");
    }
}
