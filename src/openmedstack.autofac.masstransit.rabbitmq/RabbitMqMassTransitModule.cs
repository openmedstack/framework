namespace OpenMedStack.Autofac.MassTransit.RabbitMq
{
    using global::Autofac;
    using global::MassTransit;
    using Newtonsoft.Json;
    using OpenMedStack.Autofac.MassTransit.CloudEvents;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class RabbitMqMassTransitModule<TConfiguration> : Module
        where TConfiguration : DeploymentConfiguration
    {
        private readonly TConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqMassTransitModule{T}"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="TConfiguration"/> containing the configuration values.</param>
        public RabbitMqMassTransitModule(TConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
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
            return Bus.Factory.CreateUsingRabbitMq(
                    rmq =>
                    {
                        rmq.Host(
                            _configuration.ServiceBus,
                            s =>
                            {
                                s.Heartbeat(_configuration.Timeout);
                                s.RequestedConnectionTimeout(_configuration.Timeout);
                                if (_configuration.ClusterHosts.Length > 0)
                                {
                                    s.UseCluster(
                                        cluster =>
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
                        rmq.UseCloudEvents(c.Resolve<JsonSerializerSettings>(), c.Resolve<IProvideTopic>());
                        rmq.ConfigureBus(c, _configuration, retryPolicy);
                    })
                .AttachObservers(c);
        }
    }
}
