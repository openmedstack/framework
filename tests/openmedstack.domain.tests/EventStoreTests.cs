// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventStoreTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Blob type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Domain.Tests;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenMedStack.Autofac;
using OpenMedStack.Autofac.MassTransit;
using OpenMedStack.Autofac.NEventstore;
using OpenMedStack.Autofac.NEventstore.InMemory;
using Xbehave;

public abstract class EventStoreTests
{
    protected Chassis<DeploymentConfiguration> Service = null!;

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
                                    { new Regex(".+"), new Uri("loopback://localhost/test") }
                                }
                            })
                        .DefinedIn(GetType().Assembly)
                        .AddAutofacModules((c, _) => new TestModule(c))
                        .UsingNEventStore()
                        .UsingInMemoryEventDispatcher(TimeSpan.FromSeconds(0.25))
                        .UsingInMemoryEventStore()
                        .UsingInMemoryMassTransit()
                        .Build();
                    Service.Start();
                })
            .Teardown(
                async () =>
                {
                    await Service.DisposeAsync().ConfigureAwait(false);
                });
    }
}
