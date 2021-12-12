namespace OpenMedStack.Autofac.MassTransit.Azure
{
    using System.Diagnostics.Contracts;
    using global::Autofac;
    using global::MassTransit;
    using GreenPipes;
    using OpenMedStack.Autofac.MassTransit;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class AzureServiceBusMassTransitModule : Module
    {
        private readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusMassTransitModule"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="DeploymentConfiguration"/> containing the configuration values.</param>
        public AzureServiceBusMassTransitModule(DeploymentConfiguration configuration)
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
                    c => CreateAzureBus(c, Retry.Interval(_configuration.RetryCount, _configuration.RetryInterval)))
                .As<IBusControl>()
                .As<IBus>()
                .As<IPublishEndpoint>()
                .SingleInstance();
        }

        private IBusControl CreateAzureBus(IComponentContext c, IRetryPolicy retryPolicy)
        {
            var bus = Bus.Factory.CreateUsingAzureServiceBus(
                rmq =>
                {
                    rmq.Host(
                        _configuration.ServiceBus,
                        s =>
                        {
                            //s.OperationTimeout = _configuration.Timeout;
                            //s.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                            //    _configuration.ServiceBusUsername,
                            //    _configuration.ServiceBusPassword,
                            //    TimeSpan.FromHours(8));
                        });
                    rmq.ConfigureBus(c, _configuration, retryPolicy);
                });

            return bus.AttachObservers(c);
        }
    }
}
