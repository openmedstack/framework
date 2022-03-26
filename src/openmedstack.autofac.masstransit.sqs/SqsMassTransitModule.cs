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
    public class SqsMassTransitModule : Module
    {
        private readonly DeploymentConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqsMassTransitModule"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="openmedstack.DeploymentConfiguration"/> containing the configuration values.</param>
        public SqsMassTransitModule(DeploymentConfiguration configuration)
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
            var bus = Bus.Factory.CreateUsingAmazonSqs(
                sqs =>
                {
                    var environment = _configuration.Environment ?? throw new Exception("Must set environment name");
                    sqs.MessageTopology.SetEntityNameFormatter(
                        new MessageNameFormatterEntityNameFormatter(
                            new SegmentedMessageNameFormatter(
                                environment,
                                new AmazonSqsMessageNameFormatter())));
                    sqs.Host(
                        _configuration.ServiceBus,
                        s =>
                        {
                            s.AccessKey(_configuration.ServiceBusUsername);
                            s.SecretKey(_configuration.ServiceBusPassword);
                        });
                    sqs.ConfigureBus(c, _configuration, retryPolicy);
                });

            return bus.AttachObservers(c);
        }
    }
}
