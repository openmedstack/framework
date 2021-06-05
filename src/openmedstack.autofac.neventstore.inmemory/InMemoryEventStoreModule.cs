// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryEventStoreModule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the InMemoryEventStoreModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.InMemory
{
    using global::Autofac;
    using OpenMedStack.NEventStore;
    using OpenMedStack.NEventStore.Serialization;

    public class InMemoryEventStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(
                    Wireup.Init()
                        .UsingInMemoryPersistence()
                        .UsingJsonSerialization()
                        .LinkToAutofac(builder)
                        .Build())
                .As<IStoreEvents>()
                .SingleInstance();
        }
    }
}
