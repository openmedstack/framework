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
    using System.Data.Common;
    using System.Linq;
    using global::Autofac.Core;
    using Microsoft.Extensions.Logging;
    using NEventStore.Persistence.Sql;

    public static class SqlChassisExtensions
    {
        private const string LogFilters = "LogFilters";

        public static Chassis UsingSqlEventStore(this Chassis chassis, DbProviderFactory dbProviderFactory, ISqlDialect dialect, bool compress = false)
        {
            return chassis.AddAutofacModules(
                     (c, a) => new SqlEventStoreModule(c.ConnectionString, dbProviderFactory, dialect, compress));
        }

        /// <summary>
        /// Configures the <see cref="Chassis"/> to use event sourcing.
        /// </summary>
        /// <param name="chassis">The <see cref="Chassis"/> to configure.</param>
        /// <param name="compress"><c>true</c> to compress content in database, otherwise <c>false</c>.</param>
        /// <param name="modules">The <see cref="IModule"/> to use for container configuration.</param>
        /// <param name="dbProviderFactory">The <see cref="DbProviderFactory"/> for the connection</param>
        /// <param name="dialect">The <see cref="ISqlDialect"/> specific to the database.</param>
        /// <returns>A configured instance of the <see cref="Chassis"/>.</returns>
        public static Chassis UsingSqlEventSourceBuilder(this Chassis chassis, DbProviderFactory dbProviderFactory, ISqlDialect dialect, bool compress = false, params IModule[] modules)
        {
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (configuration, assemblies) =>
                {
                    var enableConsoleLogging = (bool)chassis.Metadata.GetOrDefault(
                        ChassisExtensions.EnableConsoleLogging,
                        true)!;
                    var asm = assemblies.ToArray();
                    return new GenericSqlEventSourceService(
                        configuration,
                        dbProviderFactory,
                        dialect,
                        modules.Concat(chassis.GetModules(configuration, asm)),
                        filters as (string, LogLevel)[],
                        enableConsoleLogging,
                        compress,
                        asm);
                });
        }
    }
}
