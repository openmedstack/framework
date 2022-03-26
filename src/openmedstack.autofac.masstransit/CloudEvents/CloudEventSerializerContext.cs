// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventSerializerContext.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System;
using System.Collections.Generic;
using global::MassTransit;
using global::MassTransit.Metadata;
using global::MassTransit.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class CloudEventSerializerContext : SerializerContext
{
    private readonly CloudEventEnvelope _envelope;
    private readonly JsonSerializer _jsonSerializer;

    public CloudEventSerializerContext(CloudEventEnvelope envelope, JsonSerializer serializer)
    {
        _envelope = envelope;
        _jsonSerializer = serializer;
        SupportedMessageTypes = envelope.MessageType;
    }

    //public bool HasMessageType(Type messageType) => true;

    //public bool TryGetMessage<T>(out ConsumeContext<T>? consumeContext)
    //{
    //    try
    //    {
    //        if (TryGetPayload(out T? payload))
    //        {
    //            consumeContext = new MessageConsumeContext<T>(this, payload);
    //            return true;
    //        }

    //        consumeContext = null;
    //        return false;
    //    }
    //    catch (NotSupportedException)
    //    {
    //        consumeContext = null;
    //        return false;
    //    }
    //}

    ///// <inheritdoc />
    //public bool HasPayloadType(Type payloadType) => true;

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

    public Guid? MessageId => _envelope.MessageId;
    public Guid? RequestId => _envelope.RequestId;
    public Guid? CorrelationId => _envelope.CorrelationId;
    public Guid? ConversationId => _envelope.ConversationId;
    public Guid? InitiatorId => _envelope.InitiatorId;
    public DateTime? ExpirationTime => _envelope.ExpirationTime;
    public Uri? SourceAddress => _envelope.SourceAddress;
    public Uri? DestinationAddress => _envelope.DestinationAddress;
    public Uri? ResponseAddress => _envelope.ResponseAddress;
    public Uri? FaultAddress => _envelope.FaultAddress;
    public DateTime? SentTime => _envelope.SentTime;
    public Headers Headers => _envelope.Headers;
#if DEBUG
    public HostInfo Host { get; } = new BusHostInfo(true);
#else
        public HostInfo Host { get; }
#endif
    /// <inheritdoc />
    public bool IsSupportedMessageType<T>()
        where T : class =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public bool TryGetMessage<T>(out T? payload)
        where T : class
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

        payload = null;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetMessage(Type messageType, out object? message) => throw new NotImplementedException();

    /// <inheritdoc />
    public IMessageSerializer GetMessageSerializer() => throw new NotImplementedException();

    /// <inheritdoc />
    public IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        where T : class =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public IMessageSerializer GetMessageSerializer(object message, string[] messageTypes) => throw new NotImplementedException();

    /// <inheritdoc />
    public Dictionary<string, object> ToDictionary<T>(T? message)
        where T : class =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public string[] SupportedMessageTypes { get; }

    /// <inheritdoc />
    public T? DeserializeObject<T>(object? value, T? defaultValue = default(T?))
        where T : class =>
        throw new NotImplementedException();

    /// <inheritdoc />
    T? IObjectDeserializer.DeserializeObject<T>(object? value, T? defaultValue) =>
        throw new NotImplementedException();
}
