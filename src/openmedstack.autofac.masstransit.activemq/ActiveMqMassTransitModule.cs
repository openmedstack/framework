namespace OpenMedStack.Autofac.MassTransit.ActiveMq
{
    using System;
    using global::Autofac;
    using global::MassTransit;
    using global::MassTransit.ActiveMqTransport;
    using GreenPipes;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class ActiveMqMassTransitModule : Module
    {
        private readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMqMassTransitModule"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="DeploymentConfiguration"/> containing the configuration values.</param>
        /// <exception cref="ArgumentException"></exception>
        public ActiveMqMassTransitModule(DeploymentConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.QueueName))
            {
                throw new ArgumentException("Queue name missing", nameof(configuration));
            }

            if (configuration.ServiceBus == null)
            {
                throw new ArgumentException("Service bus address missing", nameof(configuration));
            }

            _configuration = configuration;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterBusDependencies(_configuration);
            builder.Register(
                    c => CreateActiveMq(c, Retry.Interval(_configuration.RetryCount, _configuration.RetryInterval)))
                .As<IBusControl>()
                .As<IBus>()
                .As<IPublishEndpoint>()
                .SingleInstance();
        }

        private IBusControl CreateActiveMq(IComponentContext c, IRetryPolicy retryPolicy)
        {
            var bus = Bus.Factory.CreateUsingActiveMq(
                rmq =>
                {
                    rmq.Host(
                        _configuration.ServiceBus!.Host,
                        _configuration.ServiceBus.Port,
                        s =>
                        {
                            s.Password(_configuration.ServiceBusPassword);
                            s.Username(_configuration.ServiceBusUsername);
                            if (_configuration.ClusterHosts?.Length > 0)
                            {
                                s.FailoverHosts(_configuration.ClusterHosts);
                            }
                            if (_configuration.ServiceBus.Scheme == "ssl")
                            {
                                s.UseSsl();
                            }
                        });
                    rmq.ConfigureBus(c, _configuration, retryPolicy);
                });

            return bus.AttachObservers(c);
        }
    }
}
