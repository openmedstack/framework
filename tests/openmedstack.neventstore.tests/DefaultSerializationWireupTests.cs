namespace OpenMedStack.NEventStore.Tests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NEventStore;
    using NEventStore.Persistence.AcceptanceTests;
    using NEventStore.Persistence.AcceptanceTests.BDD;
    using Xunit;

    public class DefaultSerializationWireupTests
    {
        public class WhenBuildingAnEventStoreWithoutAnExplicitSerializer : SpecificationBase
        {
            private Wireup _wireup;
            private Exception _exception;
            private IStoreEvents _eventStore;

            public WhenBuildingAnEventStoreWithoutAnExplicitSerializer()
            {
                OnStart().Wait();
            }

            protected override Task Context()
            {
                _wireup = Wireup.Init().UsingInMemoryPersistence();
                return Task.CompletedTask;
            }

            protected override Task Because()
            {
                _exception = Catch.Exception(() => { _eventStore = _wireup.Build(); });

                return Task.CompletedTask;
            }

            protected override void Cleanup()
            {
                _eventStore.Dispose();
            }

            [Fact]
            public void should_not_throw_an_argument_null_exception()
            {
                // _exception.Should().NotBeOfType<ArgumentNullException>();
                _exception.Should().BeNull();
            }
        }
    }
}
