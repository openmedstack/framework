namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using CloudNative.CloudEvents;
    using global::MassTransit;
    using Newtonsoft.Json;

    public class CloudEventSerializer : IMessageSerializer
    {
        private readonly JsonSerializer _jsonSerializer;
        private readonly IProvideTopic _topicProvider;

        public CloudEventSerializer(JsonSerializerSettings settings, IProvideTopic topicProvider)
        {
            _jsonSerializer = JsonSerializer.Create(settings);
            _topicProvider = topicProvider;
        }

        public void Serialize<T>(Stream stream, SendContext<T> context) where T : class
        {
            var topic = _topicProvider.Get<T>();

            var cloudEvent = new CloudEvent(CloudEventsSpecVersion.Default)
            {
                Data = context.Message,
                Source = context.SourceAddress,
                Id = (context.MessageId ?? Guid.NewGuid()).ToString(),
                Type = ContentType.ToString(),
                Time = context.SentTime,
                DataContentType = "application/json+" + topic,
                Subject = topic
            };

            if (context.Message is ICorrelate correlation && !string.IsNullOrWhiteSpace(correlation.CorrelationId))
            {
                cloudEvent[CloudEventAttribute.CreateExtension("correlationid", CloudEventAttributeType.String)] =
                    correlation.CorrelationId;
            }

            if (context.RequestId.HasValue)
            {
                cloudEvent[CloudEventAttribute.CreateExtension("requestid", CloudEventAttributeType.String)] =
                    context.RequestId.Value.ToString();
            }

            if (context.ConversationId.HasValue)
            {
                cloudEvent[CloudEventAttribute.CreateExtension("conversationid", CloudEventAttributeType.String)] =
                    context.ConversationId.Value.ToString();
            }

            if (context.InitiatorId.HasValue)
            {
                cloudEvent[CloudEventAttribute.CreateExtension("initiatorid", CloudEventAttributeType.String)] =
                    context.InitiatorId.Value.ToString();
            }

            if (context.FaultAddress != null)
            {
                cloudEvent[CloudEventAttribute.CreateExtension("faultaddress", CloudEventAttributeType.String)] =
                    context.FaultAddress.AbsoluteUri;
            }

            if (context.ResponseAddress != null)
            {
                cloudEvent[CloudEventAttribute.CreateExtension("responseaddress", CloudEventAttributeType.String)] =
                    context.ResponseAddress.AbsoluteUri;
            }

            if (context.DestinationAddress != null)
            {
                cloudEvent[CloudEventAttribute.CreateExtension("destinationaddress", CloudEventAttributeType.String)] =
                    context.DestinationAddress.AbsoluteUri;
            }

            var formatter = new CustomEventFormatter(_jsonSerializer);
            var bytes = formatter.EncodeStructuredModeMessage(cloudEvent, out var contentType);
            //contentType.Parameters.Add("topic", topic);
            context.ContentType = contentType;
            stream.Write(bytes.Span);
        }

        public ContentType ContentType
        {
            get;
            set;
        } = new("application/cloudevents+json");
    }
}
