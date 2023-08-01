namespace OpenMedStack.Autofac.MassTransit.Sqs
{
    using System;
    using global::Autofac;
    using global::MassTransit;
    using global::MassTransit.AmazonSqsTransport;
    using OpenMedStack.Autofac.MassTransit;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the Autofac module for configuring message endpoints.
    /// </summary>
    public class SqsMassTransitModule<TConfiguration> : Module
        where TConfiguration : DeploymentConfiguration
    {
        private readonly TConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqsMassTransitModule{T}"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="TConfiguration"/> containing the configuration values.</param>
        public SqsMassTransitModule(TConfiguration configuration)
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
                    c => CreateSqsBus(c, Retry.Interval(_configuration.RetryCount, _configuration.RetryInterval)))
                .As<IBusControl>()
                .As<IBus>()
                .As<IPublishEndpoint>()
                .SingleInstance();
        }

        private IBusControl CreateSqsBus(IComponentContext c, IRetryPolicy retryPolicy)
        {
            return Bus.Factory.CreateUsingAmazonSqs(
                    sqs =>
                    {
                        var environment = _configuration.Environment
                                          ?? throw new Exception("Must set environment name");
                        sqs.Host(
                            _configuration.ServiceBus,
                            s =>
                            {
                                s.AccessKey(_configuration.ServiceBusUsername);
                                s.SecretKey(_configuration.ServiceBusPassword);
                            });
                        sqs.ConfigureBus(c, _configuration, retryPolicy);
                        sqs.MessageTopology.SetEntityNameFormatter(
                            new MessageNameFormatterEntityNameFormatter(
                                new SegmentedMessageNameFormatter(
                                    environment,
                                    new AmazonSqsMessageNameFormatter())));
                    })
                .AttachObservers(c);
        }
    }
}
