namespace OpenMedStack.Autofac.NEventStore.Dispatcher.Polling;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using global::Autofac;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Autofac.NEventstore.Modules;
using OpenMedStack.Domain;
using OpenMedStack.ReadModels;
using Module = global::Autofac.Module;

internal class CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker, TConfiguration> : Module
    where TEventTracker : ITrackEventCheckpoints
    where TCommandTracker : ITrackCommandCheckpoints
    where TReadModelTracker : ITrackReadModelCheckpoints
    where TConfiguration : DeploymentConfiguration
{
    private readonly Assembly[] _assemblies;

    /// <summary>
    /// Initializes a new instance of an <see cref="EventStoreModule"/> class.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for read model updaters.</param>
    public CommitDispatcherModule(IEnumerable<Assembly> assemblies)
    {
        _assemblies = assemblies.DistinctBy((a, b) => a.FullName == b.FullName).ToArray();
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(_assemblies)
            .AssignableTo<IUpdateReadModel>()
            .AsSelf()
            .As<IUpdateReadModel>();

        builder.RegisterType<ReadModelCommitDispatcher>().As<IReadModelCommitDispatcher>().SingleInstance();
        builder.RegisterType<CommandCommitDispatcher<TConfiguration>>().As<ICommandCommitDispatcher>().SingleInstance();
        builder.RegisterType<EventCommitDispatcher>().As<IEventCommitDispatcher>().SingleInstance();
        builder.RegisterType<TEventTracker>().As<ITrackEventCheckpoints>().SingleInstance();
        builder.RegisterType<TCommandTracker>().As<ITrackCommandCheckpoints>().SingleInstance();
        builder.RegisterType<TReadModelTracker>().As<ITrackReadModelCheckpoints>().SingleInstance();
        builder.RegisterType<ReadModelUpdater>().As<IReadModelUpdater>().SingleInstance();
    }
}
