// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventSerializerFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the CloudEventSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System.Net.Mime;
using global::MassTransit;

public class CloudEventSerializerFactory : ISerializerFactory
{
    private readonly CloudEventSerializer _serializer;
    private readonly CloudEventDeserializer _deserializer;

    public CloudEventSerializerFactory(CloudEventSerializer serializer, CloudEventDeserializer deserializer)
    {
        _serializer = serializer;
        _deserializer = deserializer;
    }

    /// <inheritdoc />
    public IMessageSerializer CreateSerializer() => _serializer;

    /// <inheritdoc />
    public IMessageDeserializer CreateDeserializer() => _deserializer;

    /// <inheritdoc />
    public ContentType ContentType => _deserializer.ContentType;
}
