namespace OpenMedStack.Autofac.NEventStore.Sql.Tests
{
    using OpenMedStack.Commands;
    using OpenMedStack.Domain;
    using OpenMedStack.Events;

    public class TestSaga : SagaBase<BaseEvent, DomainCommand>
    {
        /// <inheritdoc />
        public TestSaga(string id) : base(id)
        {
        }

        private void Apply(TestEvent obj)
        {
        }
    }
}