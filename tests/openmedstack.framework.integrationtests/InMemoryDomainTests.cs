namespace OpenMedStack.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack;
    using OpenMedStack.Autofac;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.NEventstore;
    using OpenMedStack.Autofac.NEventstore.InMemory;
    using Xunit;

    public class InMemoryDomainTests : IDisposable
    {
        private readonly Chassis _chassis;

        public InMemoryDomainTests()
        {
            var config = new DeploymentConfiguration
            {
                TenantPrefix = "Test",
                QueueName = "Test",
                Services = new Dictionary<Regex, Uri>
                {
                    {new Regex(".+"), new Uri("loopback://localhost/Test") }
                },
                RetryCount = 5,
                RetryInterval = TimeSpan.FromSeconds(3),
                Timeout = TimeSpan.FromSeconds(5),
                Name = "Test",
                ServiceBus = new Uri("loopback://localhost/")
            };

            _chassis = Chassis.From(config)
                .DefinedIn(typeof(TestAggregate).Assembly)
                .DisableDefaultConsoleLogging()
                .UsingNEventStore()
                .UsingInMemoryEventDispatcher(TimeSpan.FromSeconds(1))
                .UsingInMemoryMassTransit()
                .UsingInMemoryEventSourceBuilder(new TestModule());
        }

        [Fact]
        public async Task WhenSendingCommandToValidAggregateThenEventIsRaised()
        {
            var cts = new CancellationTokenSource();
            var waitHandle = new ManualResetEvent(false);
            var wf = _chassis.Start(cts.Token).ConfigureAwait(false);
            using var s = _chassis.Subscribe(_ => waitHandle.Set());
            await _chassis.Send(new TestCommand(Guid.NewGuid().ToString(), 0), cts.Token).ConfigureAwait(false);

            var success = waitHandle.WaitOne(TimeSpan.FromSeconds(Debugger.IsAttached ? 5 : 50));

            Assert.True(success);
            cts.Cancel();
            await wf;
        }

        [Fact]
        public async Task WhenStartTaskIsCancelledThenChassisExits()
        {
            var tokenSource = new CancellationTokenSource();
            using var wf = _chassis.Start(tokenSource.Token);
            tokenSource.Cancel();
            await wf.ConfigureAwait(false);
        }

        [Fact]
        public async Task CanWriteToLogger()
        {
            var tokenSource = new CancellationTokenSource();
            var wf = _chassis.Start(tokenSource.Token).ConfigureAwait(false);
            var logger = _chassis.Resolve<ILogger<TestAggregate>>();

            logger.LogCritical("test");
            logger.LogError("test");
            logger.LogWarning("test");
            logger.LogInformation("test");
            logger.LogDebug("test");
            logger.LogTrace("test");
            tokenSource.Cancel();
            await wf;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _chassis?.Dispose();
        }
    }
}