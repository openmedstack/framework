// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the SqlChassisExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Sql
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using global::Autofac.Core;
    using Microsoft.Extensions.Logging;
    using NEventStore.Persistence.Sql;

    public static class SqlChassisExtensions
    {
        private const string CompressionKey = "Compression";
        private const string DialectKey = "Dialect";
        private const string DbFactoryKey = "DbFactory";
        private const string LogFilters = "LogFilters";

        /// <summary>
        /// Compresses the event data in the database.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to configure.</param>
        /// <param name="usingCompression"><c>true</c> to use compression, otherwise <c>false</c>.</param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis UsingDatabaseCompression(this Chassis chassis, bool usingCompression = true)
        {
            chassis.Metadata[CompressionKey] = usingCompression;
            return chassis;
        }

        public static Chassis UsingDbFactory(this Chassis chassis, DbProviderFactory dbProviderFactory)
        {
            chassis.Metadata[DbFactoryKey] = dbProviderFactory;
            return chassis;
        }

        public static Chassis UsingDialect(this Chassis chassis, ISqlDialect dialect)
        {
            chassis.Metadata[DialectKey] = dialect;
            return chassis;
        }

        /// <summary>
        /// Configures the <see cref="Chassis"/> to use event sourcing.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to configure.</param>
        /// <param name="modules">The <see cref="IModule"/> to use for container configuration.</param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis UsingSqlEventSourceBuilder(this Chassis chassis, params IModule[] modules)
        {
            if (!chassis.Metadata.ContainsKey(DialectKey))
            {
                throw new NullReferenceException("Dialect must be specified");
            }

            if (!chassis.Metadata.ContainsKey(DbFactoryKey))
            {
                throw new NullReferenceException("Database factory must be specified");
            }

            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (configuration, assemblies) =>
                {
                    var compress = (bool)chassis.Metadata.GetOrDefault(
                        CompressionKey,
                        false)!;
                    var enableConsoleLogging = (bool)chassis.Metadata.GetOrDefault(
                        ChassisExtensions.EnableConsoleLogging,
                        true)!;
                    var asm = assemblies.ToArray();
                    return new GenericSqlEventSourceService(
                        configuration,
                        (DbProviderFactory)chassis.Metadata[DbFactoryKey],
                        (ISqlDialect)chassis.Metadata[DialectKey],
                        modules.Concat(chassis.GetModules(configuration, asm)),
                        filters as (string, LogLevel)[],
                        enableConsoleLogging,
                        compress,
                        asm);
                });
        }
    }
}
