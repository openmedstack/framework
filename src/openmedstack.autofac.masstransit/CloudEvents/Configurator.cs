namespace OpenMedStack.Autofac.MassTransit.CloudEvents
{
    using System;
    using System.Net.Mime;
    using Newtonsoft.Json;

    internal class Configurator : IConfigurator
    {
        private readonly Serializer _serializer;
        private readonly Deserializer _deserializer;

        public Configurator(Serializer serializer, Deserializer deserializer)
        {
            _serializer = serializer;
            _deserializer = deserializer;
        }
        
        public IConfigurator WithContentType(ContentType contentType)
        {
            _serializer.ContentType =
                _deserializer.ContentType = contentType;
            
            return this;
        }

        public IConfigurator Type<T>(string type)
        {
            _deserializer.AddType<T>(type);
            _serializer.AddType<T>(type);
            
            return this;
        }

        //IConfigurator IConfigurator.WithJsonOptions(Action<JsonSerializerSettings> options)
        //{
        //    options(_serializer.Settings);
        //    options(_deserializer.Options);

        //    return this;
        //}
    }
}