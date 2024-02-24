namespace OpenMedStack.DynamoDB.Tests.Steps
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.Runtime;
    using Microsoft.Extensions.Logging.Abstractions;
    using OpenMedStack.Autofac;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.NEventstore;
    using OpenMedStack.Autofac.NEventstore.DynamoDb;
    using OpenMedStack.Autofac.NEventstore.DynamoDb.Dispatcher;
    using OpenMedStack.Events;
    using OpenMedStack.NEventStore.Abstractions;
    using Xunit;

    [Binding]
    public class DynamoDbServiceSteps
    {
        private readonly ManualResetEventSlim _waitHandle = new ManualResetEventSlim(false);
        private Chassis<DeploymentConfiguration> _chassis = null!;

        [Given(@"a running dynamodb event store service")]
        public void GivenARunningDynamodbEventStoreService()
        {
            var credentials = new BasicAWSCredentials("blah", "blah");
            var serviceUrl = new Uri("http://localhost:8000");
            _chassis = Chassis.From(new DeploymentConfiguration { QueueName = "default" })
                .UsingInMemoryMassTransit()
                .UsingNEventStore()
                .UsingDynamoDbEventStore(
                    credentials,
                    serviceUrl)
                .UsingDynamoDbDispatcher(credentials, serviceUrl)
                .Build();
            _chassis.Start();
        }

        [Given(@"a subscription to the event store")]
        public void GivenASubscriptionToTheEventStore()
        {
            _chassis.Subscribe(e => _waitHandle.Set());
        }

        [When(@"an event is published to the event store")]
        public async Task WhenAnEventIsPublishedToTheEventStore()
        {
            var persistence = _chassis.Resolve<ICommitEvents>();
            var eventStream = OptimisticEventStream.Create(
                "default",
                Guid.NewGuid().ToString("N"),
                NullLogger<OptimisticEventStream>.Instance);
            eventStream.Add(new EventMessage(new TestEvent()));
            await persistence.Commit(eventStream);
        }

        [Then(@"the event is received by the subscription")]
        public void ThenTheEventIsReceivedByTheSubscription()
        {
            Assert.True(_waitHandle.Wait(TimeSpan.FromSeconds(1)));
        }
    }

    public record TestEvent() :
    DomainEvent("TestEvent", 1, DateTimeOffset.UtcNow)
    {
    }
}
