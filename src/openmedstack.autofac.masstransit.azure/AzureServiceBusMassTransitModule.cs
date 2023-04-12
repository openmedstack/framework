namespace OpenMedStack.Autofac.MassTransit.Azure
{
    using System.Diagnostics.Contracts;
    using global::Autofac;
    using global::MassTransit;
    using Newtonsoft.Json;
    using OpenMedStack.Autofac.MassTransit;
    using OpenMedStack.Autofac.MassTransit.CloudEvents;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class AzureServiceBusMassTransitModule<TConfiguration> : Module
        where TConfiguration : DeploymentConfiguration
    {
        private readonly TConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusMassTransitModule{T}"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="TConfiguration"/> containing the configuration values.</param>
        public AzureServiceBusMassTransitModule(TConfiguration configuration)
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
            return Bus.Factory.CreateUsingAzureServiceBus(
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
                    rmq.UseCloudEvents(c.Resolve<JsonSerializerSettings>(), c.Resolve<IProvideTopic>());
                    rmq.ConfigureBus(c, _configuration, retryPolicy);
                }).AttachObservers(c);
        }
    }
}
