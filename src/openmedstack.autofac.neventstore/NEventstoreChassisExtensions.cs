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
        /// <param name="chassis">The <see cref="Chassis"/> to register for.</param>
        /// <returns>The updated <see cref="Chassis"/> instance.</returns>
        public static Chassis UsingNEventStore(this Chassis chassis) =>
            chassis.AddAutofacModules((c, a) => new EventStoreModule(() => GetDetector(chassis), a.ToArray()));

        /// <summary>
        /// Registers dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis"/> instance.</returns>
        public static Chassis UsingSingleEventDispatcher<TEventTracker, TCommandTracker, TReadModelTracker>(
            this Chassis chassis,
            TimeSpan pollingInterval)
            where TEventTracker : ITrackEventCheckpoints
            where TCommandTracker : ITrackCommandCheckpoints
            where TReadModelTracker : ITrackReadModelCheckpoints =>
            chassis.AddAutofacModules(
                    (c, a) => new CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker>(
                        a,
                        pollingInterval))
                .AddAutofacModules((c, a) => new CompositePollingClientModule(pollingInterval));

        /// <summary>
        /// Registers dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis"/> instance.</returns>
        public static Chassis UsingSeparateEventDispatcher<TEventTracker, TCommandTracker, TReadModelTracker>(
            this Chassis chassis,
            TimeSpan pollingInterval)
            where TEventTracker : ITrackEventCheckpoints
            where TCommandTracker : ITrackCommandCheckpoints
            where TReadModelTracker : ITrackReadModelCheckpoints =>
            chassis.AddAutofacModules(
                    (c, a) => new CommitDispatcherModule<TEventTracker, TCommandTracker, TReadModelTracker>(
                        a,
                        pollingInterval))
                .AddAutofacModules((c, a) => new SeparatePollingClientModule(pollingInterval));


        /// <summary>
        /// Registers in memory dispatcher types for NEventStore.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to register for.</param>
        /// <param name="pollingInterval">The <see cref="TimeSpan"/> between polls.</param>
        /// <returns>The updated <see cref="Chassis"/> instance.</returns>
        public static Chassis UsingInMemoryEventDispatcher(this Chassis chassis, TimeSpan pollingInterval) =>
            chassis
                .UsingSingleEventDispatcher<InMemoryEventCheckpointTracker, InMemoryCommandCheckpointTracker,
                    InMemoryReadModelCheckpointTracker>(pollingInterval);

        /// <summary>
        /// Configures how to handle event conflicts.
        /// </summary>
        /// <typeparam name="TCommitted">The <see cref="Type"/> of the committed event.</typeparam>
        /// <typeparam name="TUncommitted">The <see cref="Type"/> of the uncommitted event.</typeparam>
        /// <param name="chassis">The <see cref="Chassis"/> to configure.</param>
        /// <param name="handler">The <see cref="Func{TCommitted,TUncommited,TResult}"/> to use for handling event conflicts.</param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis ResolvingConflictsWith<TCommitted, TUncommitted>(
            this Chassis chassis,
            Func<TCommitted, TUncommitted, bool> handler)
            where TCommitted : DomainEvent where TUncommitted : DomainEvent
        {
            var detector = GetDetector(chassis);
            detector.Register<TCommitted, TUncommitted>((committed, uncommitted) => handler(committed, uncommitted));
            return chassis;
        }

        private static ConflictDetector GetDetector(Chassis chassis)
        {
            if (!chassis.Metadata.ContainsKey(ConflictDetectorKey))
            {
                chassis.Metadata[ConflictDetectorKey] = new ConflictDetector();
            }

            return (ConflictDetector) chassis.Metadata[ConflictDetectorKey];
        }
    }
}
