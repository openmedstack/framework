// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the domain configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using global::Autofac;
using global::MassTransit;
using Newtonsoft.Json;
using OpenMedStack.Commands;
using OpenMedStack.Events;
using Module = global::Autofac.Module;

/// <summary>
/// Defines the domain configuration.
/// </summary>
internal class DomainModule<TConfiguration> : Module
    where TConfiguration : DeploymentConfiguration
{
    private readonly TConfiguration _configuration;
    private readonly IEnumerable<Assembly> _sourceAssemblies;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainModule{T}"/> class.
    /// </summary>
    /// <param name="configuration">The <see cref="TConfiguration"/> to read configuration values from.</param>
    /// <param name="sourceAssemblies"></param>
    public DomainModule(TConfiguration configuration, IEnumerable<Assembly> sourceAssemblies)
    {
        _configuration = configuration;
        _sourceAssemblies = sourceAssemblies.DistinctBy((a, b) => a.FullName == b.FullName).ToArray();
    }

    /// <summary>
    /// Override to add registrations to the container.
    /// </summary>
    /// <remarks>
    /// Note that the ContainerBuilder parameter is unique to this module.
    /// </remarks>
    /// <param name="builder">
    /// The builder through which components can be registered.
    /// </param>
    protected override void Load(ContainerBuilder builder)
    {
        Contract.Assume(builder != null);

        base.Load(builder);

        builder.RegisterInstance(
                new JsonSerializerSettings
                {
                    MetadataPropertyHandling = MetadataPropertyHandling.Default,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.None
                })
            .AsSelf()
            .IfNotRegistered(typeof(JsonSerializerSettings));
        builder.RegisterType<ConfigurationTenantProvider>()
            .As<IProvideTenant>()
            .SingleInstance()
            .IfNotRegistered(typeof(IProvideTenant));
        builder.Register<IProvideTopic>(
                ctx => new StaticTopicProvider(ctx.Resolve<IProvideTenant>(), _configuration.TopicMap))
            .SingleInstance();
        builder.RegisterType<EnvironmentTopicNameFormatter>().As<IEntityNameFormatter>().SingleInstance();
        builder.RegisterGeneric(typeof(MassTransitCommandSubscriber<>))
            .As(typeof(ISubscribeCommands<>))
            .SingleInstance();
        builder.RegisterGeneric(typeof(MassTransitEventSubscriber<>)).As(typeof(ISubscribeEvents<>)).SingleInstance();

        var assemblyTypes = _sourceAssemblies.SelectMany(TypeSelector)
            .Where(t => !t.IsAbstract)
            .ToArray();

        var commandHandlers = assemblyTypes
            .Where(x => typeof(IHandleCommands).IsAssignableFrom(x))
            .ToArray();

        builder.RegisterTypes(commandHandlers).AsSelf().AsImplementedInterfaces();

        var commandConsumers =
            commandHandlers
                .SelectMany(CollectionSelector)
                .Where(
                    t => t.IsGenericType
                     && typeof(IHandleCommands<>).IsAssignableFrom(t.GetGenericTypeDefinition()))
                .Select(t => t.GetGenericArguments()[0])
                .Distinct()
                .Select(Selector)
                .ToArray();
        builder.RegisterTypes(commandConsumers).AsSelf().AsImplementedInterfaces();

        var messageHandlerTypes = _sourceAssemblies.SelectMany(TypeSelector)
            .Where(Predicate)
            .ToArray();

        builder.RegisterTypes(messageHandlerTypes).AsSelf().AsImplementedInterfaces();

        var eventHandlers = messageHandlerTypes
            .SelectMany(CollectionSelector)
            .Where(IsBaseEventHandler)
            .Select(t => t.GetGenericArguments()[0])
            .Distinct()
            .Select(Func)
            .ToArray();

        builder.RegisterTypes(eventHandlers).AsSelf().AsImplementedInterfaces();
        return;

        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Types available at runtime.")]
        IEnumerable<Type> TypeSelector([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Assembly a) =>
            a.GetTypes();

        IEnumerable<Type> CollectionSelector(Type t) => t.GetInterfaces();

        bool Predicate(Type t) => !t.IsAbstract && t.GetInterfaces().Any(IsBaseEventHandler);

        [UnconditionalSuppressMessage(
            "AOT",
            "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Generic type is not known at compile time.")]
        Type Func(Type t) => typeof(BaseEventConsumer<>).MakeGenericType(t);

        [UnconditionalSuppressMessage(
            "AOT",
            "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Generic type is not known at compile time.")]
        Type Selector(Type t) => typeof(CommandConsumer<>).MakeGenericType(t);
    }

    private static bool IsBaseEventHandler(Type t) =>
        t.IsGenericType && typeof(IHandleEvents<>).IsAssignableFrom(t.GetGenericTypeDefinition());
}
