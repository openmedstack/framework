// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the extension methods for <see cref="Chassis{TConfiguration}" />.
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
    /// Defines the extension methods for <see cref="Chassis{TConfiguration}"/>.
    /// </summary>
    public static class ChassisExtensions
    {
        internal const string EnableConsoleLogging = "EnableConsoleLogging";
        private const string LogFilters = "LogFilters";

        public static Chassis<TConfiguration> DisableDefaultConsoleLogging<TConfiguration>(
            this Chassis<TConfiguration> chassis)
            where TConfiguration : DeploymentConfiguration
        {
            chassis.Metadata[EnableConsoleLogging] = false;

            return chassis;
        }

        public static Chassis<TConfiguration> AddLogFilter<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            params (string, LogLevel)[] filter)
            where TConfiguration : DeploymentConfiguration
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
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to configure.</param>
        /// <param name="moduleFactory">The <see cref="Func{TResult}"/> to use to build the container modules.</param>
        /// <returns>A configured instance of the <see cref="Chassis{TConfiguration}"/>.</returns>
        public static Chassis<TConfiguration> Build<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            Func<TConfiguration, IEnumerable<Assembly>, IEnumerable<IModule>> moduleFactory)
            where TConfiguration : DeploymentConfiguration
        {
            var enableConsoleLogging = (bool)chassis.Metadata.GetOrDefault(EnableConsoleLogging, true);
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (configuration, assemblies) => new AutofacService<TConfiguration>(
                    configuration,
                    enableConsoleLogging,
                    filters as (string, LogLevel)[],
                    moduleFactory(configuration, assemblies).ToArray()));
        }

        /// <summary>
        /// Configures the system to use a generic container setup.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis{TConfiguration}"/> to configure.</param>
        /// <param name="modules">The <see cref="IModule"/> to include in the configuration.</param>
        /// <returns>A configured instance of the <see cref="Chassis{TConfiguration}"/>.</returns>
        public static Chassis<TConfiguration> Build<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            params IModule[] modules)
            where TConfiguration : DeploymentConfiguration
        {
            var enableConsoleLogging = (bool)chassis.Metadata.GetOrDefault(EnableConsoleLogging, true);
            var found = (chassis.Metadata.TryGetValue(LogFilters, out var filters));
            return chassis.UsingCustomBuilder(
                (c, a) => new AutofacService<TConfiguration>(
                    c,
                    enableConsoleLogging,
                    found ? ((string, LogLevel)[])filters! : Array.Empty<(string, LogLevel)>(),
                    modules.Concat(chassis.GetModules(c, a)).ToArray()));
        }
    }
}
