// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSqlEventSourceService.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the GenericSqlEventSourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using global::Autofac.Core;
    using Microsoft.Extensions.Logging;
    using NEventStore.Persistence.Sql;

    public class GenericSqlEventSourceService : AutofacService
    {
        public GenericSqlEventSourceService(
            DeploymentConfiguration manifest,
            DbProviderFactory dbProviderFactory,
            ISqlDialect dialect,
            IEnumerable<IModule> modules,
            (string, LogLevel)[]? filters = null,
            bool enableConsoleLogging = true,
            bool compress = true,
            params Assembly[] assemblies)
            : base(
                manifest,
                enableConsoleLogging,
                filters,
                new IModule[]
                    {
                        new SqlEventStoreModule(
                            manifest.ConnectionString ?? throw new NullReferenceException(),
                            dbProviderFactory,
                            dialect,
                            compress)
                    }.Concat(modules)
                    .ToArray())
        {
            Contract.Assume(assemblies != null);
        }

        ~GenericSqlEventSourceService()
        {
            Dispose(false);
        }
    }
}
