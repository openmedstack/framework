namespace OpenMedStack.Domain.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xbehave;
    using Xunit;

    public class SagaTests : EventStoreTests
    {
        [Scenario]
        public void SagaEventsAreNotRepublished()
        {
            "When a saga handles an event".x(
                async () =>
                {
                    var repository = Service.Resolve<ISagaRepository>();
                    var sagaId = Guid.NewGuid().ToString();
                    var saga = await repository.GetById<TestSaga>(sagaId).ConfigureAwait(false);

                    saga.Transition(new TestEvent(Guid.NewGuid().ToString(), 0, DateTimeOffset.UtcNow, sagaId));

                    await repository.Save(saga).ConfigureAwait(false);
                });

            "and polling client has polled".x(
                async () => await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false));

            "and it is sent on command bus".x(
                () =>
                {
                    var store = Service.Resolve<TestDataStore>();
                    Assert.Equal(1, store.Commands);
                });
        }

        [Scenario(DisplayName = "Transitioning side effects")]
        public void WhenTransitioningThenHasUncommittedEvent()
        {
            ISaga saga = null!;

            "When loading a saga".x(async () =>
            {
                var repository = Service.Resolve<ISagaRepository>();
                var sagaId = Guid.NewGuid().ToString();
                saga = await repository.GetById<TestSaga>(sagaId).ConfigureAwait(false);
            });

            "When transitioning".x(
                () => { saga.Transition(new TestEvent(Guid.NewGuid().ToString(), 0, DateTimeOffset.UtcNow)); });

            "Then has uncommitted events".x(
                () =>
                {
                    Assert.NotEmpty(saga.GetUncommittedEvents());
                });
        }

        [Scenario]
        public void WhenTransitioningWithKnownEventThenHandlesEvent()
        {
            ISaga saga = null!;

            "When loading a saga".x(async () =>
            {
                var repository = Service.Resolve<ISagaRepository>();
                var sagaId = Guid.NewGuid().ToString();
                saga = await repository.GetById<TestSaga>(sagaId).ConfigureAwait(false);
            });

            "When transitioning".x(
                () => { saga.Transition(new TestEvent(Guid.NewGuid().ToString(), 0, DateTimeOffset.UtcNow)); });

            "Then event is handled".x(() => { Assert.True(((TestSaga)saga).EventHandled); });
        }
    }
}