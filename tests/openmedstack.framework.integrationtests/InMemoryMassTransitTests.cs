namespace OpenMedStack.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OpenMedStack;
    using OpenMedStack.Autofac;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.NEventstore.InMemory;
    using Xunit;

    public class InMemoryMassTransitTests : IDisposable
    {
        private readonly Chassis _chassis;

        public InMemoryMassTransitTests()
        {
            var config = new DeploymentConfiguration
            {
                TenantPrefix = "Test",
                QueueName = "Test",
                Services = new Dictionary<Regex, Uri>(),
                RetryCount = 5,
                RetryInterval = TimeSpan.FromSeconds(3),
                Timeout = TimeSpan.FromSeconds(5),
                Name = "Test",
                ServiceBus = new Uri("loopback://localhost/Test")
            };
            _chassis = Chassis.From(config)
                .UsingInMemoryMassTransit()
                .AddLogFilter(("Microsoft", LogLevel.Warning))
                .UsingInMemoryEventSourceBuilder();
        }

        [Fact]
        public async Task WhenSubscribingToServiceEventsThenReceivesPublishedEvents()
        {
            var cts = new CancellationTokenSource();
            var waitHandle = new ManualResetEvent(false);
            var wf = _chassis.Start(cts.Token).ConfigureAwait(false);
            using (_chassis.Subscribe(_ => waitHandle.Set()))
            {
                await _chassis.Publish(new TestEvent(), cts.Token).ConfigureAwait(false);

                var success = waitHandle.WaitOne(TimeSpan.FromSeconds(5));

                Assert.True(success);
            }
            cts.Cancel();
            await wf;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _chassis?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
