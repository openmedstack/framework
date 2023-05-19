namespace OpenMedStack.Autofac.MassTransit
{
    using System.Collections.Generic;
    using System.Linq;
    using global::Autofac;
    using global::MassTransit;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;
    using Newtonsoft.Json;
    using OpenMedStack.Autofac.MassTransit.CloudEvents;

    internal static class BusExtensions
    {
        public static void RegisterBusDependencies<TConfiguration>(
            this ContainerBuilder builder,
            TConfiguration configuration)
            where TConfiguration : DeploymentConfiguration
        {
            builder.RegisterInstance(new FixedServicesLookup(configuration.Services))
                .As<ILookupServices>()
                .SingleInstance();
            builder.RegisterType<BusSpy>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<BusStarter>().As<IBootstrapSystem>().SingleInstance();
            builder.RegisterType<CommandBus<TConfiguration>>().As<IRouteCommands>();
            builder.RegisterType<MassTransitEventBus>().As<IPublishEvents>();
        }

        public static void ConfigureBus<T, TConfiguration>(
            this T sbc,
            IComponentContext c,
            TConfiguration configuration,
            IRetryPolicy retryPolicy)
            where T : class, IBusFactoryConfigurator
            where TConfiguration : DeploymentConfiguration
        {
            sbc.ConfigureJson(c);
            sbc.ReceiveEndpoint(
                configuration.QueueName,
                e =>
                {
                    e.UseInMemoryOutbox();
                    e.UseRetry(r => r.SetRetryPolicy(_ => retryPolicy));
                    e.RegisterConsumers(c);
                });

            sbc.MessageTopology.SetEntityNameFormatter(c.Resolve<IEntityNameFormatter>());
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

        public static void ConfigureJson(this IBusFactoryConfigurator configurator, IComponentContext c)
        {
            var settings = c.Resolve<JsonSerializerSettings>();
            var converters = c.Resolve<IEnumerable<JsonConverter>>().ToArray();

            foreach (var converter in converters)
            {
                settings.Converters.Add(converter);
            }

            configurator.UseCloudEvents(settings, c.Resolve<IProvideTopic>());
        }
    }
}
