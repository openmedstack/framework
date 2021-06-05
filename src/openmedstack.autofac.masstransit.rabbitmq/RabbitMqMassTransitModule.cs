namespace OpenMedStack.Autofac.MassTransit.RabbitMq
{
    using System.Diagnostics.Contracts;
    using global::Autofac;
    using GreenPipes;
    using global::MassTransit;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class RabbitMqMassTransitModule : Module
    {
        private readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqMassTransitModule"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="openmedstack.DeploymentConfiguration"/> containing the configuration values.</param>
        public RabbitMqMassTransitModule(DeploymentConfiguration configuration)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(configuration.QueueName));

            _configuration = configuration;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            Contract.Assume(builder != null);

            base.Load(builder);
            builder.RegisterBusDependencies(_configuration);
            builder.Register(
                    c => CreateRabbitMq(c, Retry.Interval(_configuration.RetryCount, _configuration.RetryInterval)))
                .As<IBusControl>()
                .As<IBus>()
                .As<IPublishEndpoint>()
                .SingleInstance();
        }

        private IBusControl CreateRabbitMq(IComponentContext c, IRetryPolicy retryPolicy)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(
                rmq =>
                {
                    rmq.Host(
                        _configuration.ServiceBus,
                        s =>
                        {
                            if (_configuration.ClusterHosts?.Length > 0)
                            {
                                s.UseCluster(cluster =>
                                {
                                    foreach (var clusterHost in _configuration.ClusterHosts)
                                    {
                                        cluster.Node(clusterHost);
                                    }
                                });
                            }
                            s.Password(_configuration.ServiceBusPassword);
                            s.Username(_configuration.ServiceBusUsername);
                        });
                    rmq.ConfigureBus(c, _configuration, retryPolicy);
                });

            return bus.AttachObservers(c);
        }
    }
}
