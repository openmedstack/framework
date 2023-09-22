namespace OpenMedStack.Framework.IntegrationTests;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Logging;
using OpenMedStack;
using OpenMedStack.Autofac;
using OpenMedStack.Autofac.MassTransit;
using OpenMedStack.Autofac.NEventstore;
using openmedstack.autofac.neventstore.dispatcher.polling;
using OpenMedStack.Autofac.NEventstore.InMemory;
using Xunit;

public class InMemoryDomainTests
{
    private readonly Chassis<DeploymentConfiguration> _chassis;

    public InMemoryDomainTests()
    {
        _chassis = Chassis.From(
                config =>
                    new DeploymentConfiguration
                    {
                        TenantPrefix = config["DeploymentConfiguration:Tenant"]!,
                        QueueName = config["DeploymentConfiguration:Queue"]!,
                        Services = new Dictionary<Regex, Uri>
                        {
                            { new Regex(".+"), new Uri("loopback://localhost/Test") }
                        },
                        RetryCount = 5,
                        RetryInterval = TimeSpan.FromSeconds(3),
                        Timeout = TimeSpan.FromSeconds(5),
                        Name = config["DeploymentConfiguration:Name"]!,
                        ServiceBus = new Uri(config["DeploymentConfiguration:ServiceBus:Host"]!)
                    },
                b => b.AddConfiguration(
                    new ConfigurationRoot(
                        new List<IConfigurationProvider>
                        {
                            new
                                MemoryConfigurationProvider(
                                    new MemoryConfigurationSource
                                    {
                                        InitialData = new Dictionary<string, string?>
                                        {
                                            ["DeploymentConfiguration:Tenant"] = "Test",
                                            ["DeploymentConfiguration:Queue"] = "Test",
                                            ["DeploymentConfiguration:Name"] = "Test",
                                            ["DeploymentConfiguration:ServiceBus:Host"] = "loopback://localhost/",
                                        }
                                    })
                        })))
            .DefinedIn(typeof(TestAggregate).Assembly)
            .DisableDefaultConsoleLogging()
            .UsingNEventStore()
            .UsingInMemoryEventStore()
            .UsingInMemoryEventDispatcher(TimeSpan.FromSeconds(0.25))
            .UsingInMemoryMassTransit()
            .Build(new TestModule());
    }

    [Fact]
    public async Task WhenSendingCommandToValidAggregateThenEventIsRaised()
    {
        try
        {
            using var waitHandle = new ManualResetEvent(false);
            _chassis.Start();
            using var s = _chassis.Subscribe(_ => waitHandle.Set());
            await _chassis.Send(new TestCommand(Guid.NewGuid().ToString(), 0), CancellationToken.None)
;

            var success = waitHandle.WaitOne(TimeSpan.FromSeconds(Debugger.IsAttached ? 5 : 50));

            Assert.True(success);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            await _chassis.DisposeAsync();
        }
    }

    [Fact]
    public async Task WhenStartTaskIsCancelledThenChassisExits()
    {
        try
        {
            _chassis.Start();
            await _chassis.DisposeAsync();
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Fact]
    public async Task CanWriteToLogger()
    {
        try
        {
            _chassis.Start();
            var logger = _chassis.Resolve<ILogger<TestAggregate>>();

            logger.LogCritical("test");
            logger.LogError("test");
            logger.LogWarning("test");
            logger.LogInformation("test");
            logger.LogDebug("test");
            logger.LogTrace("test");
            await _chassis.DisposeAsync();
        }
        catch (OperationCanceledException)
        {
        }
    }
}
