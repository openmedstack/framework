namespace openmedstack.masstransit.tests;

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using CloudNative.CloudEvents.NewtonsoftJson;
using OpenMedStack;
using OpenMedStack.Autofac;
using OpenMedStack.Autofac.MassTransit;
using Xunit;

public class CloudEventTests
{
    [Fact]
    public async Task CanRoundTrip()
    {
        using var tokenSource =
            new CancellationTokenSource(TimeSpan.FromSeconds(Debugger.IsAttached ? 300 : 3));
        var waitHandle = new ManualResetEventSlim(false);
        var configuration = new DeploymentConfiguration { TenantPrefix = "Test", QueueName = "test" };
        var chassis = Chassis.From(configuration)
            .DefinedIn(typeof(TestEvent).Assembly)
            .UsingInMemoryMassTransit()
            .AddAutofacModules((_, _) => new BusTestModule(waitHandle))
            .Build();
        await using var __ = chassis.ConfigureAwait(false);
        chassis.Start();
        var publishTask = chassis.Publish(new TestEvent("test", 1, DateTimeOffset.UtcNow), tokenSource.Token);
        var handled = waitHandle.Wait(TimeSpan.FromSeconds(Debugger.IsAttached ? 300 : 3));
        await publishTask.ConfigureAwait(false);

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
    ""version"": 1,
    ""timestamp"": ""2021-12-09T20:12:05.2096652+01:00"",
    ""correlationId"": null
}
        }";

        var deserializer = new JsonEventFormatter<TestEvent>();
        var deserialized = deserializer.DecodeStructuredModeMessage(Encoding.UTF8.GetBytes(json), null, null);

        Assert.NotNull(deserialized);
    }
}

internal class BusTestModule : Module
{
    private readonly ManualResetEventSlim _waitHandle;

    public BusTestModule(ManualResetEventSlim waitHandle)
    {
        _waitHandle = waitHandle;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterInstance(_waitHandle);
    }
}