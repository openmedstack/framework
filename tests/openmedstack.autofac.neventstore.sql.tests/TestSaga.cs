namespace OpenMedStack.Autofac.NEventStore.Sql.Tests;

using OpenMedStack.Domain;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.Abstractions.Persistence;

public class TestSaga : SagaBase
{
    /// <inheritdoc />
    public TestSaga(string id, IEventStream stream)
        : base(id, stream)
    {
    }

    private void Apply(TestEvent obj)
    {
    }
}
