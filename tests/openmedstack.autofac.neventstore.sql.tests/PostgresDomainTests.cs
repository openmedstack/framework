namespace OpenMedStack.Autofac.NEventStore.Sql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging.Abstractions;
    using Npgsql;
    using OpenMedStack;
    using OpenMedStack.Autofac.MassTransit.ActiveMq;
    using OpenMedStack.Autofac.NEventstore;
    using OpenMedStack.Autofac.NEventstore.Sql;
    using OpenMedStack.NEventStore.Persistence.Sql.SqlDialects;
    using Xunit;
    using Xunit.Abstractions;

    public class PostgresDomainTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly Chassis _chassis;

        public PostgresDomainTests(ITestOutputHelper output)
        {
            _output = output;
            var config = new DeploymentConfiguration
            {
                TenantPrefix = "Test",
                QueueName = "Test",
                Services = new Dictionary<Regex, Uri> { { new Regex(".+"), new Uri("activemq://localhost:61616/Test") } },
                ConnectionString =
                    "Server=localhost;Port=5432;Database=openmedstack;User Id=openmedstack;Password=openmedstack;",
                RetryCount = 5,
                RetryInterval = TimeSpan.FromSeconds(3),
                Timeout = TimeSpan.FromSeconds(5),
                Name = "Test",
                ServiceBus = new Uri("tcp://localhost:61616"),
                ServiceBusPassword = "admin",
                ServiceBusUsername = "admin"
            };

            _chassis = Chassis.From(config)
                .DefinedIn(typeof(TestAggregate).Assembly)
                .UsingNEventStore()
                .UsingInMemoryEventDispatcher(TimeSpan.FromSeconds(0.1))
                .UsingMassTransitOverActiveMq()
                .UsingSqlEventStore(NpgsqlFactory.Instance, new PostgreSqlDialect(NullLogger.Instance))
                .Build(new TestModule());
        }

        [Fact]
        public async Task WhenSendingCommandToValidAggregateThenEventIsRaised()
        {
            var waitHandle = new ManualResetEvent(false);
            await using var wf = _chassis.Start();
            using (_chassis.Subscribe(_ => waitHandle.Set()))
            {
                var response = await _chassis.Send(new TestCommand(Guid.NewGuid().ToString(), 0)).ConfigureAwait(false);

                Assert.Null(response.FaultMessage);

                var success = waitHandle.WaitOne(TimeSpan.FromSeconds(5));

                Assert.True(success);
            }
            
            try
            {
                waitHandle.Dispose();
            }
            catch (Exception exception)
            {
                _output.WriteLine(exception.Message);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _chassis?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
