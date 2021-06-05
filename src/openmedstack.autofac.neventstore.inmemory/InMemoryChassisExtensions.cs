// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the InMemoryChassisExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.InMemory
{
    using System.Linq;
    using global::Autofac.Core;
    using Microsoft.Extensions.Logging;

    public static class InMemoryChassisExtensions
    {
        private const string LogFilters = "LogFilters";

        /// <summary>
        /// Registers in memory storage of events.
        /// </summary>
        /// <param name="chassis"></param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis UsingInMemoryEventStore(this Chassis chassis)
        {
            return chassis.AddAutofacModules((c, a) => new InMemoryEventStoreModule());
        }

        /// <summary>
        /// Configures the <see cref="Chassis"/> to use event sourcing.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to configure.</param>
        /// <param name="modules">The <see cref="IModule"/> to use for container configuration.</param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis UsingInMemoryEventSourceBuilder(this Chassis chassis, params IModule[] modules)
        {
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (configuration, assemblies) =>
                {
                    var enableConsoleLogging = (bool) chassis.Metadata.GetOrDefault(
                        ChassisExtensions.EnableConsoleLogging,
                        true);
                    var asm = assemblies.ToArray();
                    var allModules = modules.Concat(chassis.GetModules(configuration, asm)).ToArray();
                    return new GenericInMemoryEventSourceService(
                        allModules,
                        chassis.Configuration,
                        enableConsoleLogging,
                        filters as (string, LogLevel)[],
                        asm);
                });
        }
    }
}
