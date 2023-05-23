namespace OpenMedStack.Autofac;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using global::Autofac.Core;

public static class AutofacServiceExtensions
{
    private const string AutofacModules = "AutofacModules";

    public static Chassis<TConfiguration> AddAutofacModules<TConfiguration>(
        this Chassis<TConfiguration> chassis,
        params Func<TConfiguration, IEnumerable<Assembly>, IModule>[] modules)
        where TConfiguration : DeploymentConfiguration
    {
        var moduleList = InnerGetModules(chassis);
        moduleList.AddRange(modules);

        return chassis;
    }

    public static IEnumerable<IModule> GetModules<TConfiguration>(
        this Chassis<TConfiguration> chassis,
        TConfiguration configuration,
        IEnumerable<Assembly> assemblies)
        where TConfiguration : DeploymentConfiguration
    {
        return InnerGetModules(chassis).Select(x => x(configuration, assemblies));
    }

    private static IList<Func<TConfiguration, IEnumerable<Assembly>, IModule>>
        InnerGetModules<TConfiguration>(Chassis<TConfiguration> chassis)
        where TConfiguration : DeploymentConfiguration
    {
        if (!chassis.Metadata.ContainsKey(AutofacModules))
        {
            chassis.Metadata.Add(
                AutofacModules,
                new List<Func<TConfiguration, IEnumerable<Assembly>, IModule>>());
        }

        return chassis.Metadata[AutofacModules] as
                IList<Func<TConfiguration, IEnumerable<Assembly>, IModule>>
         ?? Array.Empty<Func<TConfiguration, IEnumerable<Assembly>, IModule>>();
    }
}