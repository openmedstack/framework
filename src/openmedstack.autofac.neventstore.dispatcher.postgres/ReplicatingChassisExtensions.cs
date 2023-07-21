// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace openmedstack.autofac.nevenstore.dispatcher.postgres
{
    using OpenMedStack;
    using OpenMedStack.Autofac;

    public static class ReplicatingChassisExtensions
    {
        /// <summary>
        /// Registers dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to register for.</param>
        /// <returns>The updated <see cref="Chassis{TConfiguration}"/> instance.</returns>
        public static Chassis<TConfiguration> UsingReplicationDispatcher<TConfiguration>(
            this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration =>
            chassis.AddAutofacModules((_, a) => new CommitDispatcherModule(a));
    }
}
