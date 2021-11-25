namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using global::MassTransit;
    using Newtonsoft.Json;

    public static class ConfiguratorExtensions
    {
        public static IConfigurator UseCloudEvents(this IBusFactoryConfigurator cfg, JsonSerializerSettings settings)
        {
            var deserializer = new Deserializer(settings);
            cfg.AddMessageDeserializer(deserializer.ContentType, () => deserializer);

            var serializer = new Serializer(settings);
            cfg.SetMessageSerializer(() => serializer);

            return new Configurator(serializer, deserializer);
        }

        public static IConfigurator UseCloudEvents(this IReceiveEndpointConfigurator cfg, JsonSerializerSettings settings)
        {
            var deserializer = new Deserializer(settings);
            cfg.AddMessageDeserializer(deserializer.ContentType, () => deserializer);

            var serializer = new Serializer(settings);
            cfg.SetMessageSerializer(() => serializer);

            return new Configurator(serializer, deserializer);
        }
    }
}