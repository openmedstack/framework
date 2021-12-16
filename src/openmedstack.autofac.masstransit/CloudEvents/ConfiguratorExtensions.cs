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
            var serializer = JsonSerializer.Create(settings);
            var eventDeserializer = new CloudEventDeserializer(serializer);
            cfg.ClearMessageDeserializers();
            cfg.AddMessageDeserializer(eventDeserializer.ContentType, () => eventDeserializer);

            cfg.AddMessageDeserializer(eventDeserializer.ContentType, () => eventDeserializer);

            var eventSerializer = new CloudEventSerializer(serializer, topicProvider);
            cfg.SetMessageSerializer(() => eventSerializer);

            return new Configurator(eventSerializer, eventDeserializer);
        }
    }
}
