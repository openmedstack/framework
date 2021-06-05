namespace OpenMedStack.Autofac.MassTransit
{
    using System.Collections.Generic;
    using System.Linq;
    using global::Autofac;
    using global::MassTransit;
    using GreenPipes;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;
    using Newtonsoft.Json;

    internal static class BusExtensions
    {
        public static void RegisterBusDependencies(this ContainerBuilder builder, DeploymentConfiguration configuration)
        {
            builder.RegisterInstance(new FixedServicesLookup(configuration.Services))
                .As<ILookupServices>()
                .SingleInstance();
            builder.RegisterType<BusSpy>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<BusStarter>().As<IBootstrapSystem>().SingleInstance();
            builder.RegisterType<CommandBus>().As<IRouteCommands>();
            builder.RegisterType<MassTransitEventBus>().As<IPublishEvents>();
        }

        public static void ConfigureBus<T>(
            this T sbc,
            IComponentContext c,
            DeploymentConfiguration configuration,
            IRetryPolicy retryPolicy)
            where T : IBusFactoryConfigurator
        {
            //sbc.SetLoggerFactory(c.Resolve<ILoggerFactory>());
            ConfigureJson(sbc, c);
            sbc.ReceiveEndpoint(
                configuration.QueueName,
                e =>
                {
                    e.UseInMemoryOutbox();
                    e.UseRetry(r => r.SetRetryPolicy(x => retryPolicy));
                    e.RegisterConsumers(c);
                });
        }

        public static IBusControl AttachObservers(this IBusControl bus, IComponentContext c)
        {
            foreach (var observer in c.Resolve<IEnumerable<ISendObserver>>())
            {
                bus.ConnectSendObserver(observer);
            }

            foreach (var observer in c.Resolve<IEnumerable<IPublishObserver>>())
            {
                bus.ConnectPublishObserver(observer);
            }

            return bus;
        }

        private static void ConfigureJson(IBusFactoryConfigurator configurator, IComponentContext c)
        {
            configurator.UseJsonSerializer();
            var converters = c.Resolve<IEnumerable<JsonConverter>>().ToArray();
            if (converters.Length == 0)
            {
                return;
            }

            configurator.ConfigureJsonDeserializer(
                jss =>
                {
                    foreach (var converter in converters)
                    {
                        jss.Converters.Add(converter);
                    }

                    return jss;
                });
            configurator.ConfigureJsonSerializer(
                jss =>
                {
                    foreach (var converter in converters)
                    {
                        jss.Converters.Add(converter);
                    }

                    return jss;
                });
        }


    }
}