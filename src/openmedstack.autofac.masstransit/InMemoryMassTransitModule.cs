﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MassTransitModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Autofac module for configuring message endpoints.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit;

using System;
using global::Autofac;
using global::MassTransit;
using Module = global::Autofac.Module;

/// <summary>
/// Defines the Autofac module for configuring message endpoints.
/// </summary>
public class InMemoryMassTransitModule<TConfiguration> : Module
    where TConfiguration : DeploymentConfiguration
{
    private readonly TConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryMassTransitModule{T}"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="TConfiguration"/> containing the configuration values.</param>
    public InMemoryMassTransitModule(TConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.QueueName))
        {
            throw new ArgumentException("Queue name missing", nameof(configuration));
        }

        _configuration = configuration;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterBusDependencies(_configuration);
        builder.Register(
                c => CreateInMemory(c, Retry.Interval(_configuration.RetryCount, _configuration.RetryInterval)))
            .As<IBusControl>()
            .As<IBus>()
            .As<IPublishEndpoint>()
            .SingleInstance();
    }

    private IBusControl CreateInMemory(IComponentContext c, IRetryPolicy retryPolicy)
    {
        return Bus.Factory.CreateUsingInMemory(
                sbc =>
                {
                    sbc.PrefetchCount = 1;
                    sbc.ConfigureBus(c, _configuration, retryPolicy);
                })
            .AttachObservers(c);
    }
}
