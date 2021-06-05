namespace OpenMedStack.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac.Core;

    public static class AutofacServiceExtensions
    {
        private const string AutofacModules = "AutofacModules";

        public static Chassis AddAutofacModules(
            this Chassis chassis,
            params Func<DeploymentConfiguration, IEnumerable<Assembly>, IModule>[] modules)
        {
            var moduleList = InnerGetModules(chassis);
            moduleList.AddRange(modules);

            return chassis;
        }

        public static IEnumerable<IModule> GetModules(
            this Chassis chassis,
            DeploymentConfiguration configuration,
            IEnumerable<Assembly> assemblies)
        {
            return InnerGetModules(chassis).Select(x => x(configuration, assemblies));
        }

        private static IList<Func<DeploymentConfiguration, IEnumerable<Assembly>, IModule>> InnerGetModules(Chassis chassis)
        {
            if (!chassis.Metadata.ContainsKey(AutofacModules))
            {
                chassis.Metadata.Add(AutofacModules, new List<Func<DeploymentConfiguration, IEnumerable<Assembly>, IModule>>());
            }

            return chassis.Metadata[AutofacModules] as IList<Func<DeploymentConfiguration, IEnumerable<Assembly>, IModule>>
                   ?? Array.Empty<Func<DeploymentConfiguration, IEnumerable<Assembly>, IModule>>();
        }
    }
}