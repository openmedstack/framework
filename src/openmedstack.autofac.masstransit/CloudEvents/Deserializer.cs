namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using CloudNative.CloudEvents;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using global::MassTransit;
    using global::MassTransit.Context;
    using global::MassTransit.Metadata;
    using GreenPipes;
    using Newtonsoft.Json;

    public class Deserializer : IMessageDeserializer
    {
        private readonly JsonSerializerSettings _settings;
        private readonly Dictionary<string, Type> _types = new ();

        public Deserializer(JsonSerializerSettings settings)
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

            return new CloudEventContext(receiveContext, message, _types, _settings);
        }

        public ContentType ContentType
        {
            get;
            set; 
        } = new ("application/cloudevents+json");
        
        public void AddType<T>(string type) => 
            _types[type] = typeof(T);

        private class CloudEventContext : DeserializerConsumeContext
        {
            private readonly CloudEvent _cloudEvent;
            private readonly Dictionary<string, Type> _mappings;
            private readonly JsonSerializerSettings _options;

            public CloudEventContext(
                ReceiveContext receiveContext,
                CloudEvent cloudEvent,
                Dictionary<string, Type> mappings,
                JsonSerializerSettings options)
                : base(receiveContext)
            {
                _cloudEvent = cloudEvent;
                _mappings = mappings;
                _options = options;
                ConversationId = receiveContext.GetConversationId();
                CorrelationId = receiveContext.GetCorrelationId();
                InitiatorId = receiveContext.GetInitiatorId();
                MessageId = Guid.TryParse(_cloudEvent.Id, out var result)
                    ? result
                    : receiveContext.GetMessageId(Guid.NewGuid());
                RequestId = receiveContext.GetRequestId();
                Headers = receiveContext.TransportHeaders;
            }

            public override bool HasMessageType(Type messageType) =>
                true;

            public override bool TryGetMessage<T>(out ConsumeContext<T>? consumeContext)
            {
                try
                {
                    var message = _cloudEvent.ToObject<T>(_options);
                    consumeContext = message != null ? new MessageConsumeContext<T>(this, message) : null;
                    return message != null;
                }
                catch (NotSupportedException)
                {
                    consumeContext = null;
                    return false;
                }
            }

            public override Guid? MessageId
            {
                get;
            }

            public override Guid? RequestId { get; }
            public override Guid? CorrelationId { get; }
            public override Guid? ConversationId { get; }
            public override Guid? InitiatorId { get; }
            public override DateTime? ExpirationTime { get; }
            public override Uri SourceAddress => _cloudEvent.Source!;
            public override Uri? DestinationAddress { get; }
            public override Uri? ResponseAddress { get; }
            public override Uri? FaultAddress { get; }
            public override DateTime? SentTime => _cloudEvent.Time?.DateTime;
            public override Headers? Headers { get; }
            public override HostInfo? Host { get; } = new BusHostInfo(true);
            public override IEnumerable<string>? SupportedMessageTypes { get; }

            //private Type Type<T>() => 
            //    _mappings.TryGetValue(_cloudEvent.Type!, out var result) ? result : typeof(T);
        }
    }
}
