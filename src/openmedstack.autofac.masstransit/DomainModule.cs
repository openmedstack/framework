// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the domain configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using global::MassTransit;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;
    using Module = global::Autofac.Module;

    /// <summary>
    /// Defines the domain configuration.
    /// </summary>
    public class DomainModule : Module
    {
        private readonly DeploymentConfiguration _configuration;
        private readonly IEnumerable<Assembly> _sourceAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainModule"/> class.
        /// </summary>
        /// <param name="sourceAssemblies"></param>
        public DomainModule(DeploymentConfiguration configuration, IEnumerable<Assembly> sourceAssemblies)
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
            builder.Register<IProvideTopic>(
                    ctx => new EnvironmentTopicProvider(ctx.Resolve<IProvideTenant>(), _configuration.TopicMap))
                .SingleInstance();
            builder.RegisterType<EnvironmentTopicNameFormatter>().As<IEntityNameFormatter>().SingleInstance();
            builder.RegisterGeneric(typeof(MassTransitCommandSubscriber<>)).As(typeof(ISubscribeCommands<>)).SingleInstance();
            builder.RegisterGeneric(typeof(MassTransitEventSubscriber<>)).As(typeof(ISubscribeEvents<>)).SingleInstance();
            
            var assemblyTypes = _sourceAssemblies.SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract)
                .ToArray();

            var commandHandlers = assemblyTypes
                .Where(x => typeof(IHandleCommands).IsAssignableFrom(x))
                .ToArray();

            builder.RegisterTypes(commandHandlers).AsSelf().AsImplementedInterfaces();

            var commandConsumers =
                commandHandlers
                    .SelectMany(t => t.GetInterfaces())
                    .Where(
                        t => t.IsGenericType
                             && typeof(IHandleCommands<>).IsAssignableFrom(t.GetGenericTypeDefinition()))
                    .Select(t => t.GetGenericArguments()[0])
                    .Distinct()
                    .Select(t => typeof(CommandConsumer<>).MakeGenericType(t))
                    .ToArray();
            builder.RegisterTypes(commandConsumers).AsSelf().AsImplementedInterfaces();

            var messageHandlerTypes = _sourceAssemblies.SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && t.GetInterfaces().Any(IsDomainEventHandler))
                .ToArray();

            builder.RegisterTypes(messageHandlerTypes).AsSelf().AsImplementedInterfaces();

            var messageHandlers = messageHandlerTypes
                .SelectMany(t => t.GetInterfaces())
                .Where(IsDomainEventHandler)
                .Select(t => t.GetGenericArguments()[0])
                .Distinct()
                .Select(t => typeof(BaseEventConsumer<>).MakeGenericType(t))
                .ToArray();

            builder.RegisterTypes(messageHandlers).AsSelf().AsImplementedInterfaces();
        }

        private static bool IsDomainEventHandler(Type t) =>
            t.IsGenericType && typeof(IHandleEvents<>).IsAssignableFrom(t.GetGenericTypeDefinition());
    }
}
