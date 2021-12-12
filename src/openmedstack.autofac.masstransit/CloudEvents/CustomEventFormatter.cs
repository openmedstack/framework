namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using CloudNative.CloudEvents;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using Newtonsoft.Json;
    
    internal class CustomEventFormatter : JsonEventFormatter
    {
        private const string JsonMediaType = "application/json";

        public CustomEventFormatter(JsonSerializer serializer)
            : base(serializer)
        {

        }

        protected override void EncodeStructuredModeData(CloudEvent cloudEvent, JsonWriter writer)
        {
            if (cloudEvent.DataContentType?.StartsWith(JsonMediaType) == true)
            {
                writer.WritePropertyName(DataPropertyName);
                Serializer.Serialize(writer, cloudEvent.Data);
            }
            else
            {
                base.EncodeStructuredModeData(cloudEvent, writer);
            }
        }
    }
}
