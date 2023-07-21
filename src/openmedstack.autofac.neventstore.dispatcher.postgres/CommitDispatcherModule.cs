namespace openmedstack.autofac.nevenstore.dispatcher.postgres;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;
using OpenMedStack;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Autofac.NEventstore.Modules;
using OpenMedStack.Commands;
using OpenMedStack.Domain;
using OpenMedStack.Events;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.ReadModels;
using Module = global::Autofac.Module;

internal class CommitDispatcherModule : Module
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
        builder.Register(
                ctx => new ReplicatingClientSetup(
                    ctx.Resolve<IPublishEvents>(),
                    ctx.Resolve<IRouteCommands>(),
                    ctx.Resolve<ISerialize>(),
                    ctx.Resolve<ILogger<ReplicatingClientSetup>>()))
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterType<ReadModelUpdater>().As<IReadModelUpdater>().SingleInstance();
    }
}
