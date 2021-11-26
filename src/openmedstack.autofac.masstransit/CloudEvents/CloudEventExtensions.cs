namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;
    using CloudNative.CloudEvents;
    using CloudNative.CloudEvents.NewtonsoftJson;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal static class CloudEventExtensions
    {
        public static T? ToObject<T>(this CloudEvent element, JsonSerializerSettings options)
        {
            switch (element.Data)
            {
                case T item:
                    return (T?)item;
                case JToken token:
                    {
                        var serializer = JsonSerializer.Create(options);
                        return serializer.Deserialize<T>(new JTokenReader(token))!;
                    }
                default:
                    return default;
            }
        }

        public static (ContentType, ReadOnlyMemory<byte>) ToMessage(this CloudEvent cloudEvent, JsonSerializerSettings options)
        {
            var formatter = new JsonEventFormatter(JsonSerializer.Create(options));
            var bytes = formatter.EncodeStructuredModeMessage(cloudEvent, out var contentType);
            return (contentType, bytes);
        }
    }
}
