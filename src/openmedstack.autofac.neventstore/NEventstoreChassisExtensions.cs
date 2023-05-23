namespace OpenMedStack.Autofac.NEventstore
{
    using OpenMedStack.Autofac.NEventstore.Modules;
    using OpenMedStack.Domain;
    using OpenMedStack.Events;
    using System;
    using System.Linq;

    public static class NEventstoreChassisExtensions
    {
        private const string ConflictDetectorKey = "ConflictDetector";

        /// <summary>
        /// Registers types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to register for.</param>
        /// <returns>The updated <see cref="Chassis{TConfiguration}"/> instance.</returns>
        public static Chassis<TConfiguration> UsingNEventStore<TConfiguration>(this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration =>
            chassis.AddAutofacModules((_, a) => new EventStoreModule(() => GetDetector(chassis), a.ToArray()));

        /// <summary>
        /// Registers dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis{TConfiguration}"/> instance.</returns>
        public static Chassis<TConfiguration> UsingSingleEventDispatcher<TEventTracker, TCommandTracker,
            TReadModelTracker, TConfiguration>(this Chassis<TConfiguration> chassis, TimeSpan pollingInterval)
            where TEventTracker : ITrackEventCheckpoints
            where TCommandTracker : ITrackCommandCheckpoints
            where TReadModelTracker : ITrackReadModelCheckpoints
            where TConfiguration : DeploymentConfiguration =>
            chassis.AddAutofacModules(
                    (_, a) => new CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker, TConfiguration>(
                        a,
                        pollingInterval))
                .AddAutofacModules((_, _) => new CompositePollingClientModule(pollingInterval));

        /// <summary>
        /// Registers dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis{TConfiguration}"/> instance.</returns>
        public static Chassis<TConfiguration> UsingSeparateEventDispatcher<TEventTracker, TCommandTracker,
            TReadModelTracker, TConfiguration>(this Chassis<TConfiguration> chassis, TimeSpan pollingInterval)
            where TEventTracker : ITrackEventCheckpoints
            where TCommandTracker : ITrackCommandCheckpoints
            where TReadModelTracker : ITrackReadModelCheckpoints
            where TConfiguration : DeploymentConfiguration =>
            chassis.AddAutofacModules(
                    (_, a) => new CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker, TConfiguration>(
                        a,
                        pollingInterval))
                .AddAutofacModules((_, _) => new SeparatePollingClientModule(pollingInterval));


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

        /// <summary>
        /// Configures how to handle event conflicts.
        /// </summary>
        /// <typeparam name="TCommitted">The <see cref="Type"/> of the committed event.</typeparam>
        /// <typeparam name="TUncommitted">The <see cref="Type"/> of the uncommitted event.</typeparam>
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to configure.</param>
        /// <param name="handler">The <see cref="Func{TCommitted,TUncommited,TResult}"/> to use for handling event conflicts.</param>
        /// <returns>A configured instance of the <see cref="Chassis{TConfiguration}"/>.</returns>
        public static Chassis<TConfiguration> ResolvingConflictsWith<TCommitted, TUncommitted, TConfiguration>(
            this Chassis<TConfiguration> chassis,
            Func<TCommitted, TUncommitted, bool> handler)
            where TCommitted : DomainEvent
            where TUncommitted : DomainEvent
            where TConfiguration : DeploymentConfiguration
        {
            var detector = GetDetector(chassis);
            detector.Register<TCommitted, TUncommitted>((committed, uncommitted) => handler(committed, uncommitted));
            return chassis;
        }

        private static ConflictDetector GetDetector<TConfiguration>(Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration
        {
            if (!chassis.Metadata.ContainsKey(ConflictDetectorKey))
            {
                chassis.Metadata[ConflictDetectorKey] = new ConflictDetector();
            }

            return (ConflictDetector)chassis.Metadata[ConflictDetectorKey];
        }
    }
}
