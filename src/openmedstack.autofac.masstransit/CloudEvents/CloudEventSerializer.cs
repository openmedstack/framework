namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System;
using System.Net.Mime;
using CloudNative.CloudEvents;
using global::MassTransit;
using Newtonsoft.Json;

public class CloudEventSerializer : IMessageSerializer
{
    private readonly JsonSerializer _jsonSerializer;
    private readonly IProvideTopic _topicProvider;

    public CloudEventSerializer(JsonSerializer serializer, IProvideTopic topicProvider)
    {
        _jsonSerializer = serializer;
        _topicProvider = topicProvider;
    }
        
    /// <inheritdoc />
    public MessageBody GetMessageBody<T>(SendContext<T> context)
        where T : class
    {var topic = _topicProvider.Get<T>();

        var cloudEvent = new CloudEvent(CloudEventsSpecVersion.Default)
        {
            Data = context.Message,
            Source = context.SourceAddress,
            Id = (context.MessageId ?? Guid.NewGuid()).ToString(),
            Type = topic,
            Time = context.SentTime,
            DataContentType = $"application/{topic}+json",
            Subject = context.Message is ICorrelate correlation
             && !string.IsNullOrWhiteSpace(correlation.CorrelationId)
                    ? correlation.CorrelationId
                    : null
        };

        if (context.TimeToLive.HasValue)
        {
            context.Headers.Set(
                CloudEventHeaders.Expiration,
                (context.SentTime ?? DateTimeOffset.UtcNow).Add(context.TimeToLive.Value).ToUnixTimeSeconds());
        }

        if (context.RequestId.HasValue)
        {
            context.Headers.Set(CloudEventHeaders.RequestId, context.RequestId.Value.ToString(), true);
        }

        if (context.ConversationId.HasValue)
        {
            context.Headers.Set(CloudEventHeaders.ConversationId, context.ConversationId.Value.ToString(), true);
        }

        if (context.InitiatorId.HasValue)
        {
            context.Headers.Set(CloudEventHeaders.InitiatorId, context.InitiatorId.Value.ToString(), true);
        }

        if (context.FaultAddress != null)
        {
            context.Headers.Set(CloudEventHeaders.FaultAddress, context.FaultAddress.AbsoluteUri, true);
        }

        if (context.ResponseAddress != null)
        {
            context.Headers.Set(CloudEventHeaders.ResponseAddress, context.ResponseAddress.AbsoluteUri, true);
        }

        if (context.DestinationAddress != null)
        {
            context.Headers.Set(CloudEventHeaders.DestinationAddress, context.DestinationAddress.AbsoluteUri, true);
        }

        var formatter = new CustomEventFormatter(_jsonSerializer);
        var bytes = formatter.EncodeStructuredModeMessage(cloudEvent, out _);

        return new BytesMessageBody(bytes.ToArray());
    }

    public ContentType ContentType
    {
        get;
        set;
    } = new("application/cloudevents+json");
}