namespace OpenMedStack.Autofac.MassTransit.Grpc
{
    using System;
    using global::Autofac;
    using global::MassTransit;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class GrpcMassTransitModule<TConfiguration> : Module
        where TConfiguration : DeploymentConfiguration
    {
        private readonly TConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrpcMassTransitModule{T}"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="TConfiguration"/> containing the configuration values.</param>
        public GrpcMassTransitModule(TConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterBusDependencies(_configuration);
            builder.Register(
                    c => CreateGrpc(c, Retry.Interval(_configuration.RetryCount, _configuration.RetryInterval)))
                .As<IBusControl>()
                .As<IBus>()
                .As<IPublishEndpoint>()
                .SingleInstance();
        }

        private IBusControl CreateGrpc(IComponentContext c, IRetryPolicy retryPolicy)
        {
            return Bus.Factory.CreateUsingGrpc(
                    rmq =>
                    {
                        rmq.Host(
                            _configuration.ServiceBus!,
                            s =>
                            {
                                foreach (var clusterHost in _configuration.ClusterHosts)
                                {
                                    s.AddServer(new Uri(clusterHost));
                                }
                            });
                        rmq.ConfigureBus(c, _configuration, retryPolicy);
                    })
                .AttachObservers(c);
        }
    }
}