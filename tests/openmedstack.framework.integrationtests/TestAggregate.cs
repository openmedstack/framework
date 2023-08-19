namespace OpenMedStack.Framework.IntegrationTests;

using System.Diagnostics;
using OpenMedStack.Domain;

internal class TestAggregate : DefaultAggregateRoot
{
    /// <inheritdoc />
    public TestAggregate(string id, IMemento snapshot) : base($"Test-{id}", snapshot)
    {
    }

    public void DoSomething()
    {
        var evt = new TestEvent();
        RaiseEvent(evt);
    }

    private void Apply(TestEvent evt)
    {
        Trace.TraceInformation("Event applied");
    }
}
