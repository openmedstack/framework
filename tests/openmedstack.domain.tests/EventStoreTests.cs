// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventStoreTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Blob type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace OpenMedStack.Domain.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using OpenMedStack.Autofac;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.NEventstore;
    using OpenMedStack.Autofac.NEventstore.InMemory;
    using Xbehave;

    public abstract class EventStoreTests
    {
        private readonly CancellationTokenSource _cts = new();
        private ConfiguredTaskAwaitable _workflow;
        protected Chassis Service = null!;

        [Background]
        public void Background()
        {
            "Given a started service".x(
                    () =>
                    {
                        Service = Chassis
                            .From(
                                new DeploymentConfiguration
                                {
                                    TenantPrefix = "test",
                                    QueueName = "test",
                                    ServiceBus = new Uri("loopback://localhost"),
                                    Services = new Dictionary<Regex, Uri>
                                    {
                                        {new Regex(".+"), new Uri("loopback://localhost/test")}
                                    }
                                })
                            .DefinedIn(GetType().Assembly)
                            .AddAutofacModules((c, a) => new TestModule(c))
                            .UsingNEventStore()
                            .UsingInMemoryEventDispatcher(TimeSpan.FromSeconds(0.25))
                            .UsingInMemoryEventStore()
                            .UsingInMemoryMassTransit()
                            .UsingInMemoryEventSourceBuilder();
                        _workflow = Service.Start(_cts.Token).ConfigureAwait(false);
                    })
                .Teardown(
                    async () =>
                    {
                        _cts.Cancel();
                        await _workflow;
                        Service.Dispose();
                    });
        }
    }
}
