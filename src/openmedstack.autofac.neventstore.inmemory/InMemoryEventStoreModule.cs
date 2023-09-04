// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryEventStoreModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the InMemoryEventStoreModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.InMemory;

using global::Autofac;
using Microsoft.Extensions.Logging;
using OpenMedStack.NEventStore;
using OpenMedStack.NEventStore.Abstractions;
using OpenMedStack.NEventStore.Serialization;

public class InMemoryEventStoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(
                ctx => Wireup.Init(ctx.Resolve<ILoggerFactory>())
                    .UsingInMemoryPersistence()
                    .UsingJsonSerialization()
                    .Build())
            .As<IStoreEvents>()
            .SingleInstance();
        builder.Register(ctx => ctx.Resolve<IStoreEvents>().Advanced).As<IPersistStreams>().SingleInstance();
    }
}
