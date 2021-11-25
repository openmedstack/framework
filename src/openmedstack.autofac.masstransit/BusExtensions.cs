namespace OpenMedStack.Autofac.MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Autofac;
    using global::MassTransit;
    using GreenPipes;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;
    using Newtonsoft.Json;
    using OpenMedStack.Autofac.MassTransit.CloudEvents;

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
            var settings = c.ResolveOptional(typeof(JsonSerializerSettings)) as JsonSerializerSettings
                           ?? new JsonSerializerSettings
                           {
                               TypeNameHandling = TypeNameHandling.All,
                               DefaultValueHandling = DefaultValueHandling.Ignore,
                               MetadataPropertyHandling = MetadataPropertyHandling.Default,
                               TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                           };
            var converters = c.Resolve<IEnumerable<JsonConverter>>().ToArray();

            foreach (var converter in converters)
            {
                settings.Converters.Add(converter);
            }

            configurator.UseCloudEvents(settings);
        }
    }
}