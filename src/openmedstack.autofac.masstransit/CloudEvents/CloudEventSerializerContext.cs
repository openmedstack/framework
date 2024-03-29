// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventSerializerContext.cs" company="Reimers.dk">
//   Copyright � Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using global::MassTransit;
using global::MassTransit.Metadata;
using global::MassTransit.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class CloudEventSerializerContext : SerializerContext
{
    private readonly CloudEventEnvelope _envelope;
    private readonly JsonSerializer _jsonSerializer;
    private readonly IProvideTopic _topicProvider;

    public CloudEventSerializerContext(CloudEventEnvelope envelope, JsonSerializer serializer, IProvideTopic topicProvider)
    {
        _envelope = envelope;
        _jsonSerializer = serializer;
        _topicProvider = topicProvider;
        SupportedMessageTypes = envelope.MessageType;
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

    /// <inheritdoc />
    public Guid? MessageId => _envelope.MessageId;

    /// <inheritdoc />
    public Guid? RequestId => _envelope.RequestId;

    /// <inheritdoc />
    public Guid? CorrelationId => _envelope.CorrelationId;

    /// <inheritdoc />
    public Guid? ConversationId => _envelope.ConversationId;

    /// <inheritdoc />
    public Guid? InitiatorId => _envelope.InitiatorId;

    /// <inheritdoc />
    public DateTime? ExpirationTime => _envelope.ExpirationTime;

    /// <inheritdoc />
    public Uri? SourceAddress => _envelope.SourceAddress;

    /// <inheritdoc />
    public Uri? DestinationAddress => _envelope.DestinationAddress;

    /// <inheritdoc />
    public Uri? ResponseAddress => _envelope.ResponseAddress;

    /// <inheritdoc />
    public Uri? FaultAddress => _envelope.FaultAddress;

    /// <inheritdoc />
    public DateTime? SentTime => _envelope.SentTime;

    /// <inheritdoc />
    public Headers Headers => _envelope.Headers;

    /// <inheritdoc />
    public HostInfo Host { get; } = new BusHostInfo(true);

    /// <inheritdoc />
    public bool IsSupportedMessageType<T>()
        where T : class => true;

    /// <inheritdoc />
    public bool IsSupportedMessageType(Type messageType) => true;

    /// <inheritdoc />
    public bool TryGetMessage<T>(out T? payload)
        where T : class
    {
        var data = _envelope.CloudEvent.Data switch
        {
            JToken token => GetTokenValue<T>(token),
            T item => item,
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
    public bool TryGetMessage(Type messageType, [NotNullWhen(true)] out object? message)
    {
        var data = _envelope.CloudEvent.Data switch
        {
            JToken token => token.ToObject<object>(),
            { } item => item,
            _ => default
        };
        if (data != null)
        {
            message = data;
            return true;
        }

        message = null;
        return false;
    }

    /// <inheritdoc />
    public IMessageSerializer GetMessageSerializer() => new CloudEventSerializer(_jsonSerializer, _topicProvider);

    /// <inheritdoc />
    public IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        where T : class => new CloudEventSerializer(_jsonSerializer, _topicProvider);

    /// <inheritdoc />
    public IMessageSerializer GetMessageSerializer(object message, string[] messageTypes) => new CloudEventSerializer(_jsonSerializer, _topicProvider);

    /// <inheritdoc />
    public Dictionary<string, object> ToDictionary<T>(T? message)
        where T : class =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public string[] SupportedMessageTypes { get; }

    /// <inheritdoc />
    public T? DeserializeObject<T>(object? value, T? defaultValue = default)
        where T : class
    {
        if (value is null)
        {
            return defaultValue;
        }

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    T? IObjectDeserializer.DeserializeObject<T>(object? value, T? defaultValue)
        where T : struct
    {
        if (value is null)
        {
            return defaultValue;
        }

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public MessageBody SerializeObject(object? value)
    {
        return value switch
        {
            { } => SerializerInstance(value),
            _ => new EmptyMessageBody(),
        };
    }

    private MessageBody SerializerInstance(object value)
    {
        using var ms = new MemoryStream();
        using var textWriter = new StreamWriter(ms);
        using var writer = new JsonTextWriter(textWriter);
        _jsonSerializer.Serialize(writer, value, value.GetType());
        writer.Flush();
        ms.Flush();
        return new BytesMessageBody(ms.ToArray());
    }
}
