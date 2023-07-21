namespace OpenMedStack.Web.Autofac.Tests.Steps;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenMedStack.Autofac;
using OpenMedStack.Autofac.MassTransit;
using OpenMedStack.Autofac.NEventstore;
using openmedstack.autofac.neventstore.dispatcher.polling;
using OpenMedStack.Autofac.NEventstore.InMemory;
using OpenMedStack.Web.Testing;
using TechTalk.SpecFlow;

[Binding]
public partial class FeatureSteps : IAsyncDisposable
{
    private TestChassis<WebDeploymentConfiguration> _webServerService = null!;
    private HttpClient _client = null!;
    private IDisposable _subscription = null!;
    private readonly ManualResetEventSlim _waitHandle = new ManualResetEventSlim(false);

    [Given(@"a web service")]
    public void GivenAWebService()
    {
        var config = new WebDeploymentConfiguration
        {
            Name = typeof(TestStartup).Assembly.GetName().Name!,
            Services = new Dictionary<Regex, Uri>
            {
                { new Regex(".+"), new Uri("loopback://localhost/test") }
            },
            QueueName = "test",
            ServiceBus = new Uri("loopback://localhost/"),
            Timeout = TimeSpan.FromHours(1),
            RetryInterval = TimeSpan.FromSeconds(3),
            RetryCount = 3
        };
        _webServerService = Chassis.From(config)
            .AddAutofacModules((c, _) => new TestStartupModule(c))
            .UsingInMemoryEventDispatcher(TimeSpan.FromMilliseconds(200))
            .UsingInMemoryEventStore()
            .UsingNEventStore()
            .UsingInMemoryMassTransit()
            .UsingTestWebServer(
                new TestStartup(),
                new ClaimsPrincipal(
                    new ClaimsIdentity(new[] { new Claim("test", "yes") }, "test")));
        _webServerService.Start();
    }

    [Given(@"a client")]
    public void GivenAClient()
    {
        _client = _webServerService.CreateClient();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _webServerService.DisposeAsync();
    }

    [Given(@"an event subscription")]
    public void GivenAnEventSubscription()
    {
        _subscription = _webServerService.Subscribe(_ => _waitHandle.Set());
    }
}
