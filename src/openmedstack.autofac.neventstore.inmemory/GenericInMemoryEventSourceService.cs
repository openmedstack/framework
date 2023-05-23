// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericInMemoryEventSourceService.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the GenericInMemoryEventSourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.NEventstore.InMemory;

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using global::Autofac.Core;
using Microsoft.Extensions.Logging;

public class GenericInMemoryEventSourceService<TConfiguration> : AutofacService<TConfiguration>
    where TConfiguration : DeploymentConfiguration
{
    public GenericInMemoryEventSourceService(
        IEnumerable<IModule> modules,
        TConfiguration deploymentConfiguration,
        bool enableConsoleLogging = true,
        (string, LogLevel)[]? filters = null,
        params Assembly[] assemblies)
        : base(
            deploymentConfiguration,
            enableConsoleLogging,
            filters,
            modules.Concat(new InMemoryEventStoreModule()).ToArray())
    {
        Contract.Assume(assemblies != null);
    }
}
