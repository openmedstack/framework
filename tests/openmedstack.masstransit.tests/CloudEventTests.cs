namespace openmedstack.masstransit.tests
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using MassTransit;
    using Newtonsoft.Json;
    using OpenMedStack;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.MassTransit.CloudEvents;
    using Xunit;
    using Xunit.Abstractions;

    public class CloudEventTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public CloudEventTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
        
        [Fact]
        public async Task CanRoundTrip()
        {
            var configuration = new DeploymentConfiguration { TenantPrefix = "Test" };
            var waitHandle = new ManualResetEventSlim(false);
            var bus = Bus.Factory.CreateUsingInMemory(
                sbc =>
                {
                    sbc.UseCloudEvents(
                        new JsonSerializerSettings
                        {
                            MetadataPropertyHandling = MetadataPropertyHandling.Default,
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.All,
                            Formatting = Formatting.None
                        },
                        new EnvironmentTopicProvider(new ConfigurationTenantProvider(configuration)));
                    sbc.ReceiveEndpoint(
                        "test",
                        e =>
                        {
                            e.UseInMemoryOutbox();
                            e.UseRetry(r => r.SetRetryPolicy(_ => Retry.Interval(5, TimeSpan.FromMilliseconds(500))));
                            e.Consumer(() => new TestEventConsumer(waitHandle));
                        });
                });
            await bus.StartAsync();

            var publishTask = bus.Publish(new TestEvent("test", 1, DateTimeOffset.UtcNow));
            var handled = waitHandle.Wait(TimeSpan.FromSeconds(Debugger.IsAttached ? 300 : 3));
            await publishTask;

            await bus.StopAsync();

            Assert.True(handled);
        }

        [Fact]
        public void CanDeserializeFromWire()
        {
            const string json = @"{
""specversion"": ""1.0"",
""id"": ""53739e1e-3997-4872-88cf-8d9ddc9f02a1"",
""source"": ""http://localhost"",
""type"": ""application/cloudevents+json"",
""datacontenttype"": ""application/json+Sample"",
""subject"": ""Sample"",
""time"": ""2021-12-09T19:12:05.2419601Z"",
""data"": {
    ""source"": ""sample"",
    ""version"": 0,
    ""timestamp"": ""2021-12-09T20:12:05.2096652+01:00"",
    ""correlationId"": null
}
        }";
            
            var deserializer = new JsonEventFormatter<TestEvent>();
            var deserialized = deserializer.DecodeStructuredModeMessage(Encoding.UTF8.GetBytes(json), null, null);

            Assert.NotNull(deserialized);
        }
    }
}
