namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using global::MassTransit;
    using Newtonsoft.Json;

    public static class ConfiguratorExtensions
    {
        public static IConfigurator UseCloudEvents(
            this IBusFactoryConfigurator cfg,
            JsonSerializerSettings settings,
            IProvideTopic topicProvider)
        {
            var deserializer = new CloudEventDeserializer(settings);
            cfg.ClearMessageDeserializers();
            cfg.AddMessageDeserializer(deserializer.ContentType, () => deserializer);

            cfg.AddMessageDeserializer(deserializer.ContentType, () => deserializer);

            var serializer = new CloudEventSerializer(settings, topicProvider);
            cfg.SetMessageSerializer(() => serializer);

            return new Configurator(serializer, deserializer);
        }
    }
}
