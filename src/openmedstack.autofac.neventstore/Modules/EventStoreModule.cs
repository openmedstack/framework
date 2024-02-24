// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventStoreModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the module for configuring an NEventStore based event store in the project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Modules;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using global::Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Autofac.NEventstore.Repositories;
using OpenMedStack.Domain;
using OpenMedStack.NEventStore.Abstractions;

/// <summary>
/// Defines the module for configuring an NEventStore based event store in the project.
/// </summary>
public class EventStoreModule : global::Autofac.Module
{
    private readonly Assembly[] _assemblies;
    private readonly Func<IDetectConflicts> _conflictDetector;

    /// <summary>
    /// Initializes a new instance of an <see cref="EventStoreModule"/> class.
    /// </summary>
    /// <param name="conflictDetector">The <see cref="IDetectConflicts"/> to use to handle event conflicts.</param>
    /// <param name="assemblies">The list of <see cref="Assembly"/> to scan for aggregates and sagas.</param>
    public EventStoreModule(
        Func<IDetectConflicts>? conflictDetector = null,
        params Assembly[] assemblies)
    {
        _assemblies = assemblies.Concat(typeof(AggregateRootBase<>).Assembly)
            .DistinctBy((a, b) => string.Equals(a.FullName, b.FullName))
            .ToArray();
        _conflictDetector = () => conflictDetector?.Invoke() ?? new ConflictDetector();
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "Assembly scanning is required for DI.")]
    protected override void Load(ContainerBuilder builder)
    {
        Contract.Assume(builder != null);

        base.Load(builder);

        builder.RegisterAssemblyTypes(_assemblies).AssignableTo<IAggregate>().AsSelf();
        builder.RegisterAssemblyTypes(_assemblies).AssignableTo<ISaga>().AsSelf();
        builder.RegisterType<ContainerAggregateFactory>().As<IConstructAggregates>().SingleInstance();
        builder.RegisterInstance(_conflictDetector.Invoke()).As<IDetectConflicts>().SingleInstance();
        builder.RegisterType<SagaFactory>().As<IConstructSagas>().SingleInstance();
        builder.RegisterType<DefaultEventStoreRepository>().As<IRepository>().SingleInstance();
        builder.RegisterType<SagaEventStoreRepository>().As<ISagaRepository>().SingleInstance();
        builder.RegisterType<NesJsonSerializer>().As<ISerialize>().SingleInstance();
    }
}


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
