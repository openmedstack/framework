namespace OpenMedStack.Autofac.NEventstore.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Autofac.NEventstore.Domain;
    using OpenMedStack.Domain;
    using NEventStore.Persistence;
    using OpenMedStack.ReadModels;
    using Module = global::Autofac.Module;

    internal class SeparatePollingClientModule : Module
    {
        private readonly TimeSpan _pollingInterval;

        /// <summary>
        /// Initializes a new instance of an <see cref="EventStoreModule"/> class.
        /// </summary>
        /// <param name="pollingInterval">The time between event polling</param>
        public SeparatePollingClientModule(TimeSpan pollingInterval)
        {
            _pollingInterval = pollingInterval;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(
                    ctx => new PollingClientSetup<ITrackCommandCheckpoints, ICommandCommitDispatcher>(
                        ctx.Resolve<IPersistStreams>(),
                        ctx.Resolve<ICommandCommitDispatcher>(),
                        ctx.Resolve<IProvideTenant>(),
                        ctx.Resolve<ITrackCommandCheckpoints>(),
                        ctx.Resolve<ILogger<AsyncPollingClient>>(),
                        _pollingInterval))
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.Register(
                    ctx => new PollingClientSetup<ITrackEventCheckpoints, IEventCommitDispatcher>(
                        ctx.Resolve<IPersistStreams>(),
                        ctx.Resolve<IEventCommitDispatcher>(),
                        ctx.Resolve<IProvideTenant>(),
                        ctx.Resolve<ITrackEventCheckpoints>(),
                        ctx.Resolve<ILogger<AsyncPollingClient>>(),
                        _pollingInterval))
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.Register(
                    ctx => new PollingClientSetup<ITrackReadModelCheckpoints, IReadModelCommitDispatcher>(
                        ctx.Resolve<IPersistStreams>(),
                        ctx.Resolve<IReadModelCommitDispatcher>(),
                        ctx.Resolve<IProvideTenant>(),
                        ctx.Resolve<ITrackReadModelCheckpoints>(),
                        ctx.Resolve<ILogger<AsyncPollingClient>>(),
                        _pollingInterval))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }

    internal class CompositePollingClientModule : Module
    {
        private readonly TimeSpan _pollingInterval;

        /// <summary>
        /// Initializes a new instance of an <see cref="EventStoreModule"/> class.
        /// </summary>
        /// <param name="pollingInterval">The time between event polling</param>
        public CompositePollingClientModule(TimeSpan pollingInterval)
        {
            _pollingInterval = pollingInterval;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => new CompositePollingClientSetup(
                ctx.Resolve<IPersistStreams>(),
                ctx.Resolve<IProvideTenant>(),
                ctx.Resolve<ILogger<AsyncPollingClient>>(),
                _pollingInterval,
                new (ITrackCheckpoints, ICommitDispatcher)[]
                {
                    (ctx.Resolve<ITrackCommandCheckpoints>(), ctx.Resolve<ICommandCommitDispatcher>()),
                    (ctx.Resolve<ITrackEventCheckpoints>(), ctx.Resolve<IEventCommitDispatcher>()),
                    (ctx.Resolve<ITrackReadModelCheckpoints>(), ctx.Resolve<IReadModelCommitDispatcher>())
                }))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }

    internal class CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker> : Module
        where TEventTracker : ITrackEventCheckpoints
        where TCommandTracker : ITrackCommandCheckpoints
        where TReadModelTracker : ITrackReadModelCheckpoints
    {
        private readonly Assembly[] _assemblies;
        private readonly TimeSpan _pollingInterval;

        /// <summary>
        /// Initializes a new instance of an <see cref="EventStoreModule"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for read model updaters.</param>
        /// <param name="pollingInterval">The time between event polling</param>
        public CommitDispatcherModule(IEnumerable<Assembly> assemblies, TimeSpan pollingInterval)
        {
            _assemblies = assemblies.DistinctBy((a, b) => a.FullName == b.FullName).ToArray();
            _pollingInterval = pollingInterval;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assemblies)
                .AssignableTo<IUpdateReadModel>()
                .AsSelf()
                .As<IUpdateReadModel>();

            builder.RegisterType<ReadModelCommitDispatcher>().As<IReadModelCommitDispatcher>().SingleInstance();
            builder.RegisterType<CommandCommitDispatcher>().As<ICommandCommitDispatcher>().SingleInstance();
            builder.RegisterType<EventCommitDispatcher>().As<IEventCommitDispatcher>().SingleInstance();
            builder.RegisterType<TEventTracker>().As<ITrackEventCheckpoints>().SingleInstance();
            builder.RegisterType<TCommandTracker>().As<ITrackCommandCheckpoints>().SingleInstance();
            builder.RegisterType<TReadModelTracker>().As<ITrackReadModelCheckpoints>().SingleInstance();
            builder.RegisterType<ReadModelUpdater>().As<IReadModelUpdater>().SingleInstance();
        }
    }
}
