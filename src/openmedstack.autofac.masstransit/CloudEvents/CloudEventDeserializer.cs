namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;
    using System.Net.Mime;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using global::MassTransit;
    using GreenPipes;
    using Newtonsoft.Json;

    public class CloudEventDeserializer : IMessageDeserializer
    {
        private readonly JsonSerializer _serializer;
        private readonly JsonEventFormatter _formatter;

        public CloudEventDeserializer(JsonSerializer serializer)
        {
            _serializer = serializer;
            _formatter = new JsonEventFormatter();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            var envelope = ReadContext(receiveContext);
            return new CloudEventContext(receiveContext, envelope, _serializer);
        }

        public ContentType ContentType { get; set; } = new("application/cloudevents+json");

        private CloudEventEnvelope ReadContext(ReceiveContext receiveContext)
        {
            var cloudEvent = _formatter.DecodeStructuredModeMessage(
                receiveContext.GetBody(),
                receiveContext.ContentType,
                null);
            return new CloudEventEnvelope
            {
                CloudEvent = cloudEvent,
                ConversationId =
                    receiveContext.TransportHeaders.TryGetHeader(
                        CloudEventHeaders.ConversationId,
                        out var conversationHeader)
                    && Guid.TryParse(conversationHeader.ToString(), out var conversationId)
                        ? conversationId
                        : default(Guid?),
                CorrelationId =
                    Guid.TryParse(cloudEvent.Subject, out var correlation) ? correlation : default(Guid?),
                InitiatorId =
                    receiveContext.TransportHeaders.TryGetHeader(
                        CloudEventHeaders.InitiatorId,
                        out var initiatorHeader)
                    && Guid.TryParse(initiatorHeader.ToString(), out var initiatorId)
                        ? initiatorId
                        : default(Guid?),
                MessageId = receiveContext.GetMessageId(Guid.Parse(cloudEvent.Id!)),
                RequestId =
                    receiveContext.TransportHeaders.TryGetHeader(
                        CloudEventHeaders.RequestId,
                        out var requestIdHeader)
                    && Guid.TryParse(requestIdHeader.ToString(), out var requestId)
                        ? requestId
                        : default(Guid?),
                SourceAddress = cloudEvent.Source!,
                Headers = receiveContext.TransportHeaders,
                DestinationAddress =
                    receiveContext.TransportHeaders.TryGetHeader(
                        CloudEventHeaders.DestinationAddress,
                        out var destinationAddress)
                        ? new Uri(destinationAddress.ToString()!)
                        : default,
                ResponseAddress =
                    receiveContext.TransportHeaders.TryGetHeader(
                        CloudEventHeaders.ResponseAddress,
                        out var responseAddress)
                        ? new Uri(responseAddress.ToString()!)
                        : cloudEvent.Source,
                FaultAddress =
                    receiveContext.TransportHeaders.TryGetHeader(
                        CloudEventHeaders.FaultAddress,
                        out var faultAddress)
                        ? new Uri(faultAddress.ToString()!)
                        : default,
                ExpirationTime =
                    receiveContext.TransportHeaders.TryGetHeader(
                        CloudEventHeaders.Expiration,
                        out var expirationHeader)
                    && long.TryParse(expirationHeader.ToString(), out var expiration)
                        ? DateTimeOffset.FromUnixTimeSeconds(expiration).UtcDateTime
                        : default(DateTime?),
                MessageType = new[] { cloudEvent.Type! }
            };
        }
    }
}
