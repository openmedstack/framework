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
using OpenMedStack.NEventStore.Persistence.InMemory;

public class InMemoryEventStoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(
                ctx => new InMemoryPersistenceEngine(ctx.Resolve<ILogger<InMemoryPersistenceEngine>>()))
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
