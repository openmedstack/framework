// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventContext.cs" company="Reimers.dk">
//   Copyright � Reimers.dk
// </copyright>
// <summary>
//   Defines the CloudEventContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using global::MassTransit;
using global::MassTransit.Context;
using global::MassTransit.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class CloudEventContext : DeserializerConsumeContext
{
    private readonly CloudEventEnvelope _envelope;
    private readonly JsonSerializer _jsonSerializer;

    public CloudEventContext(
        ReceiveContext receiveContext,
        CloudEventEnvelope envelope,
        JsonSerializer serializer,
        IProvideTopic topicProvider)
        : base(receiveContext, new CloudEventSerializerContext(envelope, serializer, topicProvider))
    {
        _envelope = envelope;
        _jsonSerializer = serializer;
    }

    public override bool HasMessageType(Type messageType) => true;

    public override bool TryGetMessage<T>([NotNullWhen(true)] out ConsumeContext<T>? consumeContext)
    {
        try
        {
            if (TryGetPayload(out T? payload))
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
    public override bool TryGetPayload<T>([NotNullWhen(true)] out T? payload) where T : class
    {
        var data = _envelope.CloudEvent.Data switch
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

    public override Guid? MessageId => _envelope.MessageId;
    public override Guid? RequestId => _envelope.RequestId;
    public override Guid? CorrelationId => _envelope.CorrelationId;
    public override Guid? ConversationId => _envelope.ConversationId;
    public override Guid? InitiatorId => _envelope.InitiatorId;
    public override DateTime? ExpirationTime => _envelope.ExpirationTime;

    public override Uri SourceAddress =>
        _envelope.SourceAddress ?? throw new InvalidOperationException("Source address is null");

    public override Uri DestinationAddress => _envelope.DestinationAddress
     ?? throw new InvalidOperationException("Destination address is null");

    public override Uri ResponseAddress => _envelope.ResponseAddress
     ?? throw new InvalidOperationException("Destination address is null");

    public override Uri FaultAddress =>
        _envelope.FaultAddress ?? throw new InvalidOperationException("Destination address is null");

    public override DateTime? SentTime => _envelope.SentTime;
    public override Headers Headers => _envelope.Headers;
#if DEBUG
    public override HostInfo Host { get; } = new BusHostInfo(true);
#else
        public override HostInfo? Host { get; }
#endif
    public override IEnumerable<string> SupportedMessageTypes => _envelope.MessageType;
}
