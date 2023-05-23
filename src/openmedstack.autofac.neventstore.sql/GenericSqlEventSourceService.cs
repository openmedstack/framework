// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSqlEventSourceService.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the GenericSqlEventSourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.Sql;

using System.Collections.Generic;
using System.Linq;
using global::Autofac.Core;
using Microsoft.Extensions.Logging;

public class GenericSqlEventSourceService<TConfiguration> : AutofacService<TConfiguration>
    where TConfiguration : DeploymentConfiguration
{
    public GenericSqlEventSourceService(
        TConfiguration manifest,
        IEnumerable<IModule> modules,
        (string, LogLevel)[]? filters = null,
        bool enableConsoleLogging = true)
        : base(manifest, enableConsoleLogging, filters, modules.ToArray())
    {
    }
}
