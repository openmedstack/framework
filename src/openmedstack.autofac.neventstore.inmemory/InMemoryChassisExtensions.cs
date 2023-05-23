// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the InMemoryChassisExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.InMemory;

public static class InMemoryChassisExtensions
{
    /// <summary>
    /// Registers in memory storage of events.
    /// </summary>
    /// <param name="chassis"></param>
    /// <returns>A configured instance of the <see cref="Chassis{TConfiguration}"/>.</returns>
    public static Chassis<TConfiguration> UsingInMemoryEventStore<TConfiguration>(
        this Chassis<TConfiguration> chassis)
        where TConfiguration : DeploymentConfiguration
    {
        return chassis.AddAutofacModules((_, _) => new InMemoryEventStoreModule());
    }
}
