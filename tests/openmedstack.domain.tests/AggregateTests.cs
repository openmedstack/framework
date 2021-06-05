namespace OpenMedStack.Domain.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xbehave;
    using Xunit;

    public class AggregateTests : EventStoreTests
    {
        [Scenario(DisplayName = "When executing action, then read store is updated.")]
        public void ExecuteSomeScenario()
        {
            "When performing action".x(
                async () =>
                {
                    var repostory = Service.Resolve<IRepository>();
                    var aggregate = await repostory.GetById<TestAggregateRoot>("abc").ConfigureAwait(false);
                    aggregate.SomeAction();
                    await repostory.Save(aggregate).ConfigureAwait(false);
                });

            "and processing is finished".x(
                async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                    await Task.Yield();
                });

            "then data store is updated".x(
                () =>
                {
                    var store = Service.Resolve<TestDataStore>();

                    Assert.Equal(1, store.Updates);
                });
        }

        [Scenario(DisplayName = "When executing action twice, then read store is updated twice.")]
        public void ExecuteSomeScenarioTwice()
        {
            "When performing action".x(
                async () =>
                {
                    var repository = Service.Resolve<IRepository>();
                    var aggregate = await repository.GetById<TestAggregateRoot>("abc").ConfigureAwait(false);
                    aggregate.SomeAction();
                    await repository.Save(aggregate).ConfigureAwait(false);
                });

            "and processing is finished".x(
                async () =>
                {
                    await Task.Yield();
                    await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                });

            "When performing action again".x(
                async () =>
                {
                    var repository = Service.Resolve<IRepository>();
                    var aggregate = await repository.GetById<TestAggregateRoot>("abc").ConfigureAwait(false);
                    aggregate.SomeAction();
                    await repository.Save(aggregate).ConfigureAwait(false);
                });

            "and processing is finished again".x(
                async () =>
                {
                    await Task.Yield();
                    await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                });

            "then data store is updated".x(
                async () =>
                {
                    var store = Service.Resolve<TestDataStore>();
                    await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
                    Assert.Equal(2, store.Updates);
                });
        }
    }
}
