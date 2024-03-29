// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NesJsonSerializer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Modules;

using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenMedStack.NEventStore.Abstractions;

public class NesJsonSerializer : ISerialize
{
    private readonly ILogger<NesJsonSerializer> _logger;
    private readonly JsonSerializer _jsonSerializer;

    private readonly JsonSerializerSettings _serializerOptions = new()
    {
        Formatting = Formatting.None, DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateParseHandling = DateParseHandling.DateTimeOffset, FloatFormatHandling = FloatFormatHandling.String,
        TypeNameHandling = TypeNameHandling.Objects, MetadataPropertyHandling = MetadataPropertyHandling.Default,
        MissingMemberHandling = MissingMemberHandling.Ignore
    };

    public NesJsonSerializer(ILogger<NesJsonSerializer> logger)
    {
        _logger = logger;
        _jsonSerializer = JsonSerializer.Create(_serializerOptions);
    }

    public virtual void Serialize<T>(Stream output, T graph)
    {
        _logger.LogTrace("Serializing instance of {Type}", typeof(T));
        using var streamWriter = new StreamWriter(output, Encoding.UTF8);
        _jsonSerializer.Serialize(streamWriter, graph, typeof(T));
        streamWriter.Flush();
    }

    public virtual T? Deserialize<T>(Stream input)
    {
        _logger.LogTrace("Serializing stream of {Type}", typeof(T));
        using var streamReader = new StreamReader(input, Encoding.UTF8);
        using var jsonReader = new JsonTextReader(streamReader);
        return _jsonSerializer.Deserialize<T>(jsonReader);
    }

    public T? Deserialize<T>(byte[] input)
    {
        _logger.LogTrace("Serializing stream of {Type}", typeof(T));
        using var stream = new MemoryStream(input);
        var streamReader = new StreamReader(stream);
        var json = streamReader.ReadToEnd();
        var jsonReader = new JsonTextReader(new StringReader(json));
        return _jsonSerializer.Deserialize<T>(jsonReader);
    }
}
