// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the $TYPE$ type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventStore.Dispatcher.Polling
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using OpenMedStack;
    using OpenMedStack.Autofac;
    using OpenMedStack.Autofac.NEventstore;
    using OpenMedStack.Domain;

    public static class PollingChassisExtensions
    {
        /// <summary>
        /// Registers dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis{TConfiguration}"/> instance.</returns>
        public static Chassis<TConfiguration> UsingSingleEventDispatcher<
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TEventTracker,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TCommandTracker,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TReadModelTracker,
                TConfiguration>(
            this Chassis<TConfiguration> chassis,
            TimeSpan pollingInterval)
            where TEventTracker : ITrackEventCheckpoints
            where TCommandTracker : ITrackCommandCheckpoints
            where TReadModelTracker : ITrackReadModelCheckpoints
            where TConfiguration : DeploymentConfiguration =>
            chassis.AddAutofacModules(
                (_, a) =>
                    new CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker, TConfiguration>(a),
                (_, _) => new CompositePollingClientModule(pollingInterval));

        /// <summary>
        /// Registers dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis{TConfiguration}"/> instance.</returns>
        public static Chassis<TConfiguration> UsingSeparateEventDispatcher<
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TEventTracker,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TCommandTracker,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TReadModelTracker,
                TConfiguration>(
            this Chassis<TConfiguration> chassis,
            TimeSpan pollingInterval)
            where TEventTracker : ITrackEventCheckpoints
            where TCommandTracker : ITrackCommandCheckpoints
            where TReadModelTracker : ITrackReadModelCheckpoints
            where TConfiguration : DeploymentConfiguration =>
            chassis.AddAutofacModules(
                (_, a) =>
                    new CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker, TConfiguration>(a),
                (_, _) => new SeparatePollingClientModule(pollingInterval));

        /// <summary>
        /// Registers in memory dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis{TConfiguration}"/> instance.</returns>
        public static Chassis<TConfiguration> UsingInMemoryEventDispatcher<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            TimeSpan pollingInterval)
            where TConfiguration : DeploymentConfiguration =>
            chassis
                .UsingSingleEventDispatcher<InMemoryEventCheckpointTracker, InMemoryCommandCheckpointTracker,
                    InMemoryReadModelCheckpointTracker, TConfiguration>(pollingInterval);
    }
}
