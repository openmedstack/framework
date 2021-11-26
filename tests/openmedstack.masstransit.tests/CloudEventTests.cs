// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace openmedstack.masstransit.tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Context;
    using Newtonsoft.Json;
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
        public void CanSerializeCloudEvent()
        {
            using var ms = new MemoryStream();
            var evt = new TestEvent("test", 1, DateTimeOffset.UtcNow);
            var serializer = new Serializer(new JsonSerializerSettings());
            var deserializer = new JsonEventFormatter<TestEvent>();
            serializer.Serialize(ms, new MessageSendContext<TestEvent>(evt));
            var deserialized = deserializer.DecodeStructuredModeMessage(ms.ToArray(), null, null);
            ms.Position = 0;
            using var reader = new StreamReader(ms);
            var json = reader.ReadToEnd();
            _outputHelper.WriteLine(json);

            Assert.Equal("{\"specversion\":\"1.0\",\"source\":\"cloudevents:openmedstack\",\"id\":\"\"}", json);
        }

        [Fact]
        public async Task CanRoundTrip()
        {
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
                        });
                    sbc.ReceiveEndpoint(
                        "test",
                        e =>
                        {
                            e.UseInMemoryOutbox();
                            e.UseRetry(r => r.SetRetryPolicy(x => Retry.Interval(5, TimeSpan.FromMilliseconds(500))));
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
    }
}
