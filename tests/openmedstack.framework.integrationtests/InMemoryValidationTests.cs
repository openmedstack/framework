namespace OpenMedStack.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.NEventstore;
    using OpenMedStack.Autofac.NEventstore.InMemory;
    using Xunit;

    public class InMemoryValidationTests
    {
        [Fact]
        public async Task WhenNoValidCommandTopicsAreRegisteredThenStartupThrows()
        {
            var config = new DeploymentConfiguration
            {
                TenantPrefix = "Test",
                QueueName = "Test",
                Services = new Dictionary<Regex, Uri>
                {
                    {new Regex("cheese"), new Uri("loopback://localhost/Test")}
                },
                Name = "Test",
                ServiceBus = new Uri("loopback://localhost/")
            };

            var chassis = Chassis.From(config)
                .DefinedIn(typeof(TestAggregate).Assembly)
                .UsingNEventStore()
                .UsingInMemoryMassTransit()
                .UsingInMemoryEventSourceBuilder(new TestModule());

            await Assert.ThrowsAnyAsync<Exception>(() => chassis.Start()).ConfigureAwait(false);
        }

        [Fact]
        public async Task WhenNoValidCommandEndpointsAreRegisteredThenStartupThrows()
        {
            var config = new DeploymentConfiguration
            {
                TenantPrefix = "Test",
                QueueName = "Test",
                Services = new Dictionary<Regex, Uri>(),
                Name = "Test",
                ServiceBus = new Uri("loopback://localhost/")
            };

            var chassis = Chassis.From(config)
                .DefinedIn(typeof(TestAggregate).Assembly)
                .UsingNEventStore()
                .UsingInMemoryMassTransit()
                .UsingInMemoryEventSourceBuilder(new TestModule());

            await Assert.ThrowsAnyAsync<Exception>(() => chassis.Start()).ConfigureAwait(false);
        }
    }
}
