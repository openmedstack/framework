namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System.Net.Mime;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using global::MassTransit;
    using GreenPipes;
    using Newtonsoft.Json;

    public class CloudEventDeserializer : IMessageDeserializer
    {
        private readonly JsonSerializerSettings _settings;

        public CloudEventDeserializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            var formatter = new JsonEventFormatter(JsonSerializer.Create(_settings));
            var message = formatter.DecodeStructuredModeMessage(receiveContext.GetBody(), receiveContext.ContentType, null);

            return new CloudEventContext(receiveContext, message, _settings);
        }
        
        public ContentType ContentType
        {
            get;
            set; 
        } = new ("application/cloudevents+json");
    }
}
