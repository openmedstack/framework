namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using CloudNative.CloudEvents;
    using global::MassTransit;
    using Newtonsoft.Json;

    public class Serializer : IMessageSerializer
    {
        private readonly JsonSerializerSettings _settings;
        private readonly Dictionary<Type, string> _types = new();

        public Serializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public void Serialize<T>(Stream stream, SendContext<T> context) where T : class
        {
            var cloudEvent = new CloudEvent(CloudEventsSpecVersion.Default)
            {
                Data = context.Message,
                Source = context.SourceAddress ?? new Uri("cloudevents:openmedstack"),
                Id = context.MessageId.ToString(),
                Type = Type(context.Message.GetType()),
                Time = context.SentTime,
                DataContentType = "application/json",
                Subject = Type(context.Message.GetType())
            };
            cloudEvent.SetAttributeFromString("test", "value");
            var (contentType, readOnlyMemory) = cloudEvent.ToMessage(_settings);
            context.ContentType = contentType ?? ContentType;
            stream.Write(readOnlyMemory.Span);
        }

        public ContentType ContentType
        {
            get;
            set;
        } = new("application/cloudevents+json");
        
        public void AddType<T>(string type) =>
            _types[typeof(T)] = type;
    
        private string Type(Type type)
        {
            if (_types.TryGetValue(type, out var result))
            {
                return result;}

            if (type.GetCustomAttributes(typeof(CloudEventTopicAttribute), true).FirstOrDefault() is CloudEventTopicAttribute topicAttribute)
            {
                return topicAttribute.Topic;
            }

            return type.Name;
        }
    }
}
