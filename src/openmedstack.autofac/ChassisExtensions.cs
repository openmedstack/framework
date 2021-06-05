// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the extension methods for <see cref="Chassis" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac.Core;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the extension methods for <see cref="Chassis"/>.
    /// </summary>
    public static class ChassisExtensions
    {
        internal static string EnableConsoleLogging = "EnableConsoleLogging";
        internal static string LogFilters = "LogFilters";

        public static Chassis DisableDefaultConsoleLogging(this Chassis chassis)
        {
            chassis.Metadata[EnableConsoleLogging] = false;

            return chassis;
        }

        public static Chassis AddLogFilter(this Chassis chassis, params (string, LogLevel)[] filter)
        {
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            chassis.Metadata[LogFilters] = filter.Concat(
                    filters as (string, LogLevel)[] ?? Array.Empty<(string, LogLevel)>())
                .ToArray();

            return chassis;
        }

        /// <summary>
        /// Configures the system to use a generic container setup.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to configure.</param>
        /// <param name="moduleFactory">The <see cref="Func{TResult}"/> to use to build the container modules.</param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis UsingGenericBuilder(
            this Chassis chassis,
            Func<DeploymentConfiguration, IEnumerable<Assembly>, IEnumerable<IModule>> moduleFactory)
        {
            var enableConsoleLogging = (bool) chassis.Metadata.GetOrDefault(EnableConsoleLogging, true)!;
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (configuration, assemblies) => new AutofacService(
                    configuration,
                    enableConsoleLogging,
                    filters as (string, LogLevel)[],
                    moduleFactory(configuration, assemblies).ToArray()));
        }

        /// <summary>
        /// Configures the system to use a generic container setup.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to configure.</param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis UsingGenericBuilder(this Chassis chassis)
        {
            var enableConsoleLogging = (bool) chassis.Metadata.GetOrDefault(EnableConsoleLogging, true)!;
            var filters = ((string, LogLevel)[]) (chassis.Metadata[LogFilters] ?? Array.Empty<(string, LogLevel)>());
            return chassis.UsingCustomBuilder(
                (c, a) => new AutofacService(c, enableConsoleLogging, filters, chassis.GetModules(c, a).ToArray()));
        }
    }
}
