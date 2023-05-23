namespace OpenMedStack.Autofac.NEventStore.Sql.Tests;

using OpenMedStack.Domain;

public class TestSaga : SagaBase
{
    /// <inheritdoc />
    public TestSaga(string id)
        : base(id)
    {
    }

    private void Apply(TestEvent obj)
    {
    }
}
