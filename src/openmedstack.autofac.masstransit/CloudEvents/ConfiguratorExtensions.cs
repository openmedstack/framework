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
            var eventSerializer = new CloudEventSerializer(serializer, topicProvider);

            cfg.ClearSerialization();
            var cloudEventSerializerFactory = new CloudEventSerializerFactory(eventSerializer, eventDeserializer);
            cfg.AddDeserializer(cloudEventSerializerFactory);
            cfg.AddSerializer(cloudEventSerializerFactory, true);
            
            return new Configurator(eventSerializer, eventDeserializer);
        }
    }
}
