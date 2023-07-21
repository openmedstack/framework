namespace OpenMedStack.Domain.Tests.Steps;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenMedStack.Autofac;
using OpenMedStack.Autofac.MassTransit;
using OpenMedStack.Autofac.NEventstore;
using openmedstack.autofac.neventstore.dispatcher.polling;
using OpenMedStack.Autofac.NEventstore.InMemory;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public partial class FeatureSteps : IAsyncDisposable
{
    private Chassis<DeploymentConfiguration> Service = null!;
    private TestAggregateRoot _aggregate = null!;
    private ISaga _saga = null!;

    [Given(@"a started service")]
    public void GivenAStartedService()
    {
        Service = Chassis
            .From(
                new DeploymentConfiguration
                {
                    TenantPrefix = "test",
                    QueueName = "test",
                    ServiceBus = new Uri("loopback://localhost"),
                    Services = new Dictionary<Regex, Uri>
                    {
                        { new Regex(".+"), new Uri("loopback://localhost/test") }
                    }
                })
            .DefinedIn(GetType().Assembly)
            .AddAutofacModules((c, _) => new TestModule(c))
            .UsingNEventStore()
            .UsingInMemoryEventDispatcher(TimeSpan.FromSeconds(0.25))
            .UsingInMemoryEventStore()
            .UsingInMemoryMassTransit()
            .Build();
        Service.Start();
    }

    [Given(@"an aggregate root")]
    public void GivenAnAggregateRoot()
    {
        _aggregate = new TestAggregateRoot(Guid.NewGuid().ToString(), null);
    }

    [When(@"applying a known event")]
    public void WhenApplyingAKnownEvent()
    {
        ((IAggregate)_aggregate).ApplyEvent(new TestEvent(_aggregate.Id, 1, DateTimeOffset.UtcNow));
    }

    [Then(@"the event is handled")]
    public void ThenTheEventIsHandled()
    {
        Assert.True(_aggregate.EventRaised);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _aggregate.Dispose();
        await Service.DisposeAsync().ConfigureAwait(false);
    }

    [When(@"performing action")]
    public async Task WhenPerformingAction()
    {
        var repository = Service.Resolve<IRepository>();
        var aggregate = await repository.GetById<TestAggregateRoot>("abc").ConfigureAwait(false);
        aggregate.SomeAction();
        await repository.Save(aggregate).ConfigureAwait(false);
    }

    [When(@"processing is finished")]
    public async Task WhenProcessingIsFinished()
    {
        await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
    }

    [Then(@"data store is updated (.*) times")]
    public void ThenDataStoreIsUpdatedTimes(int p0)
    {
        var store = Service.Resolve<TestDataStore>();
        Assert.Equal(p0, store.Updates);
    }

    [When(@"a saga handles an event")]
    public async Task WhenASagaHandlesAnEvent()
    {
        var repository = Service.Resolve<ISagaRepository>();
        var sagaId = Guid.NewGuid().ToString();
        var saga = await repository.GetById<TestSaga>(sagaId).ConfigureAwait(false);

        saga.Transition(new TestEvent(Guid.NewGuid().ToString(), 1, DateTimeOffset.UtcNow, sagaId));

        await repository.Save(saga).ConfigureAwait(false);
    }

    [Then(@"it is sent on command bus")]
    public void ThenItIsSentOnCommandBus()
    {
        var store = Service.Resolve<TestDataStore>();
        Assert.Equal(1, store.Commands);
    }

    [When(@"loading a saga")]
    public async Task WhenLoadingASaga()
    {
        var repository = Service.Resolve<ISagaRepository>();
        var sagaId = Guid.NewGuid().ToString();
        _saga = await repository.GetById<TestSaga>(sagaId).ConfigureAwait(false);
    }

    [When(@"transitioning")]
    public void WhenTransitioning()
    {
        _saga.Transition(new TestEvent(Guid.NewGuid().ToString(), 1, DateTimeOffset.UtcNow));
    }

    [Then(@"has uncommitted events")]
    public void ThenHasUncommittedEvents()
    {
        Assert.NotEmpty(_saga.GetUncommittedEvents());
    }

    [Then(@"event is handled")]
    public void ThenEventIsHandled()
    {
        Assert.True(((TestSaga)_saga).EventHandled);
    }
}
