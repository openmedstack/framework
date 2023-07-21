namespace OpenMedStack.Autofac.NEventstore.Modules;

using System;
using global::Autofac;
using Microsoft.Extensions.Logging;
using OpenMedStack.Autofac.NEventstore.Domain;
using OpenMedStack.Domain;
using OpenMedStack.NEventStore.Abstractions;

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