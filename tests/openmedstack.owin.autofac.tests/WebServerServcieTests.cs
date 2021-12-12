namespace OpenMedStack.Web.Autofac.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenMedStack.Autofac;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.NEventstore;
    using OpenMedStack.Autofac.NEventstore.InMemory;
    using OpenMedStack.Web.Testing;
    using Xbehave;
    using Xunit;

    public static class WebServerServiceTests
    {
        public class GivenAWebServerWorkflow
        {
            private readonly CancellationTokenSource _cts = new();
            private Task _workflow = null!;
            private TestChassis _webServerService = null!;

            [Background]
            public void Background()
            {
                "Given a service".x(
                        () =>
                        {
                            var config = new DeploymentConfiguration
                            {
                                Name = typeof(GivenAWebServerWorkflow).Assembly.GetName().Name!,
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
                                .AddAutofacModules((c, a) => new TestStartupModule(c))
                                .UsingInMemoryEventDispatcher(TimeSpan.FromMilliseconds(300))
                                .UsingInMemoryEventStore()
                                .UsingNEventStore()
                                .UsingInMemoryMassTransit()
                                .UsingTestWebServer(
                                    new TestStartup(),
                                    new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim("test", "yes")}, "test")));
                            _workflow = _webServerService.Start(_cts.Token);
                        })
                    .Teardown(
                        async () =>
                        {
                            _cts.Cancel();
                            await _workflow.ConfigureAwait(false);
                            _webServerService.Dispose();
                        });
            }

            [Scenario]
            public void WhenRequestingAppplicationRootThenEventIsPublishedOnBus()
            {
                HttpClient client = null!;
                var waitHandle = new ManualResetEvent(false);
                IDisposable subscription = null!;

                "and a client".x(() => client = _webServerService.CreateClient())
                    .Teardown(() => { client!.Dispose(); });

                "and an event subscription".x(
                        () => { subscription = _webServerService.Subscribe(_ => waitHandle.Set()); })
                    .Teardown(() => { subscription?.Dispose(); });

                "when requesting application root".x(
                    async () => { _ = await client!.GetAsync("http://localhost").ConfigureAwait(false); });

                "then event is published on bus".x(
                    () =>
                    {
                        var success = waitHandle.WaitOne(TimeSpan.FromSeconds(3));
                        Assert.True(success);
                    });
            }

            [Scenario]
            public void WhenRequestingCommandPathThenCommandIsSentOnBus()
            {
                HttpClient client = null!;
                var waitHandle = new ManualResetEvent(false);
                IDisposable subscription = null!;

                "and a client".x(() => client = _webServerService.CreateClient())
                    .Teardown(() => { client?.Dispose(); });

                "and an event subscription".x(
                        () => { subscription = _webServerService.Subscribe(_ => waitHandle.Set()); })
                    .Teardown(
                        () =>
                        {
                            subscription?.Dispose();
                            waitHandle.Dispose();
                        });

                "when requesting command path".x(
                    async () =>
                    {
                        _ = await client!.GetAsync("http://localhost/commands").ConfigureAwait(false);
                    });

                "then event is published on bus".x(
                    () =>
                    {
                        var success = waitHandle.WaitOne(TimeSpan.FromSeconds(5));
                        Assert.True(success);
                    });
            }

            [Scenario]
            public void WhenRequestingAppplicationRootThenGetsOkResponse()
            {
                HttpClient client = null!;
                HttpResponseMessage response = null!;

                "and a client".x(() => client = _webServerService.CreateClient())
                    .Teardown(() => { client?.Dispose(); });

                "when requesting command path".x(
                    async () => { response = await client!.GetAsync("http://localhost").ConfigureAwait(false); });

                "then response is ok".x(() => { Assert.Equal(HttpStatusCode.OK, response!.StatusCode); });
            }
        }
    }
}
