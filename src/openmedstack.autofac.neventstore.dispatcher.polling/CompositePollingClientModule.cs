namespace OpenMedStack.Autofac.NEventStore.Dispatcher.Polling;

using System;
using global::Autofac;
using Microsoft.Extensions.Logging;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Autofac.NEventstore.Modules;
using OpenMedStack.Domain;
using OpenMedStack.NEventStore.Abstractions;

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
        builder.Register(
                ctx => new CompositePollingClientSetup(
                    ctx.Resolve<IManagePersistence>(),
                    ctx.Resolve<IProvideTenant>(),
                    ctx.Resolve<ILoggerFactory>(),
                    _pollingInterval,
                    (ctx.Resolve<ITrackCommandCheckpoints>(), ctx.Resolve<ICommandCommitDispatcher>()),
                    (ctx.Resolve<ITrackEventCheckpoints>(), ctx.Resolve<IEventCommitDispatcher>()),
                    (ctx.Resolve<ITrackReadModelCheckpoints>(), ctx.Resolve<IReadModelCommitDispatcher>())))
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
