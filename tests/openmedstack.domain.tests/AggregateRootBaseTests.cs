namespace OpenMedStack.Domain.Tests
{
    using System;
    using Xbehave;
    using Xunit;

    public static class AggregateRootBaseTests
    {
        public class GivenATestAggregateRoot
        {
            private TestAggregateRoot _aggregate = null!;

            [Background]
            public void Background()
            {
                _aggregate = new TestAggregateRoot(Guid.NewGuid().ToString(), null);
            }

            [Scenario(DisplayName = "Default event handler registration")]
            public void WhenApplyingEventThenHasHandler()
            {
                "When handling known event".x(() =>
                {
                    IAggregate a = _aggregate;
                    a.ApplyEvent(new TestEvent(a.Id, 0, DateTimeOffset.UtcNow));
                });

                "Then it is handled".x(() =>
                {
                    Assert.True(_aggregate.EventRaised);
                });
            }
        }
    }
}
