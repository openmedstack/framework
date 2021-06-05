namespace OpenMedStack.NEventStore.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Logging;
    using Newtonsoft.Json;

    internal class NesJsonSerializer : ISerialize
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(NesJsonSerializer));

        private readonly IEnumerable<Type> _knownTypes = new[] { typeof(List<EventMessage>), typeof(Dictionary<string, object>) };

        private readonly JsonSerializer _typedSerializer = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly JsonSerializer _untypedSerializer = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public NesJsonSerializer(params Type[] knownTypes)
        {
            if (knownTypes != null && knownTypes.Length > 0)
            {
                _knownTypes = knownTypes;
            }

            foreach (var type in _knownTypes)
            {
                Logger.Debug(SerializerMessages.RegisteringKnownType, type);
            }
        }

        public virtual void Serialize<T>(Stream output, T graph)
        {
            Logger.Verbose(Messages.SerializingGraph, typeof(T));
            using var streamWriter = new StreamWriter(output, Encoding.UTF8);
            Serialize(new JsonTextWriter(streamWriter), graph);
        }

        public virtual T? Deserialize<T>(Stream input)
        {
            Logger.Verbose(Messages.DeserializingStream, typeof(T));
            using var streamReader = new StreamReader(input, Encoding.UTF8);
            return Deserialize<T>(new JsonTextReader(streamReader));
        }

        protected virtual void Serialize<T>(JsonWriter writer, T graph)
        {
            using (writer)
            {
                GetSerializer(typeof(T)).Serialize(writer, graph);
            }
        }

        protected virtual T? Deserialize<T>(JsonReader reader)
        {
            var type = typeof(T);

            using (reader)
            {
                var item = GetSerializer(type).Deserialize(reader, type);
                return item == null ? default : (T) item;
            }
        }

        protected virtual JsonSerializer GetSerializer(Type typeToSerialize)
        {
            if (_knownTypes.Contains(typeToSerialize))
            {
                Logger.Verbose(SerializerMessages.UsingUntypedSerializer, typeToSerialize);
                return _untypedSerializer;
            }

            Logger.Verbose(SerializerMessages.UsingTypedSerializer, typeToSerialize);
            return _typedSerializer;
        }
    }
}