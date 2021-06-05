// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MassTransitModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Autofac module for configuring message endpoints.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit
{
    using System;
    using global::Autofac;
    using GreenPipes;
    using global::MassTransit;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class InMemoryMassTransitModule : Module
    {
        private readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryMassTransitModule"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="DeploymentConfiguration"/> containing the configuration values.</param>
        public InMemoryMassTransitModule(DeploymentConfiguration configuration)
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
            return Bus.Factory
                .CreateUsingInMemory(sbc => { sbc.ConfigureBus(c, _configuration, retryPolicy); })
                .AttachObservers(c);
        }
    }
}
