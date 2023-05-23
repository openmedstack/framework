namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System.Net.Mime;

internal class Configurator : IConfigurator
{
    private readonly CloudEventSerializer _serializer;
    private readonly CloudEventDeserializer _deserializer;

    public Configurator(CloudEventSerializer serializer, CloudEventDeserializer deserializer)
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
}