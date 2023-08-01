namespace OpenMedStack.Autofac.NEventstore;

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
    /// Configures how to handle event conflicts.
    /// </summary>
    /// <typeparam name="TCommitted">The <see cref="Type"/> of the committed event.</typeparam>
    /// <typeparam name="TUncommitted">The <see cref="Type"/> of the uncommitted event.</typeparam>
    /// <typeparam name="TConfiguration">The <see cref="Type"/> of the system configuration.</typeparam>
    /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to configure.</param>
    /// <param name="handler">The <see cref="Func{TCommitted,TUncommited,TResult}"/> to use for handling event conflicts.</param>
    /// <returns>A configured instance of the <see cref="Chassis{TConfiguration}"/>.</returns>
    public static Chassis<TConfiguration> ResolvingConflictsWith<TCommitted, TUncommitted, TConfiguration>(
        this Chassis<TConfiguration> chassis,
        Func<TCommitted, TUncommitted, bool> handler)
        where TCommitted : BaseEvent
        where TUncommitted : BaseEvent
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
