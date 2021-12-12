// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventContext.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the CloudEventContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CloudNative.CloudEvents;
using global::MassTransit;
using global::MassTransit.Context;
using global::MassTransit.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    internal class CloudEventContext : DeserializerConsumeContext
    {
        private readonly CloudEvent _cloudEvent;
        private readonly JsonSerializer _jsonSerializer;

        public CloudEventContext(ReceiveContext receiveContext, CloudEvent cloudEvent, JsonSerializerSettings options)
            : base(receiveContext)
        {
            _cloudEvent = cloudEvent;
            _jsonSerializer = JsonSerializer.Create(options);
            ConversationId = receiveContext.GetConversationId();
            CorrelationId = receiveContext.GetCorrelationId() ?? cloudEvent.GetGuid("correlationid");
            InitiatorId = receiveContext.GetInitiatorId();
            MessageId = receiveContext.GetMessageId(Guid.Parse(_cloudEvent.Id!));
            RequestId = receiveContext.GetRequestId() ?? cloudEvent.GetGuid("requestid");
            SourceAddress = _cloudEvent.Source!;
            Headers = receiveContext.TransportHeaders;
            DestinationAddress = _cloudEvent.GetUri("destinationaddress");
            ResponseAddress = _cloudEvent.GetUri("responseaddress") ?? _cloudEvent.Source;
            FaultAddress = _cloudEvent.GetUri("faultaddress");

            SupportedMessageTypes = new[] { _cloudEvent.Subject! };
        }

        public override bool HasMessageType(Type messageType) => true;

        public override bool TryGetMessage<T>(out ConsumeContext<T>? consumeContext)
        {
            try
            {
                if (TryGetPayload(out T payload))
                {
                    consumeContext = new MessageConsumeContext<T>(this, payload);
                    return true;
                }

                consumeContext = null;
                return false;
            }
            catch (NotSupportedException)
            {
                consumeContext = null;
                return false;
            }
        }

        /// <inheritdoc />
        public override bool HasPayloadType(Type payloadType) => true;

        /// <inheritdoc />
        public override bool TryGetPayload<T>(out T payload)
        {
            var data = (_cloudEvent.Data) switch
            {
                T item => item,
                JToken token => GetTokenValue<T>(token),
                _ => default
            };
            if (data != null)
            {
                payload = data;
                return true;
            }

            return base.TryGetPayload(out payload);
        }

        private T? GetTokenValue<T>(JToken token)
            where T : class
        {
            try
            {
                return _jsonSerializer.Deserialize<T>(new JTokenReader(token))!;
            }
            catch
            {
                return default;
            }
        }

        public override Guid? MessageId { get; }
        public override Guid? RequestId { get; }
        public override Guid? CorrelationId { get; }
        public override Guid? ConversationId { get; }
        public override Guid? InitiatorId { get; }
        public override DateTime? ExpirationTime { get; }
        public override Uri SourceAddress { get; }
        public override Uri? DestinationAddress { get; }
        public override Uri? ResponseAddress { get; }
        public override Uri? FaultAddress { get; }
        public override DateTime? SentTime { get { return _cloudEvent.Time?.DateTime; } }
        public override Headers? Headers { get; }
        public override HostInfo? Host { get; } = new BusHostInfo(true);
        public override IEnumerable<string>? SupportedMessageTypes { get; }
    }
}
