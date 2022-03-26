namespace OpenMedStack.Web.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Autofac.Core;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Autofac;

    public static class WebServerChassisExtensions
    {
        private const string UrlBindingKey = "WebUrlBindings";
        private const string LogFilters = "LogFilters";

        public static Chassis BindToUrls(this Chassis chassis, params string[] urls)
        {
            chassis.GetUrlBindings().AddRange(urls);
            return chassis;
        }

        public static Chassis UsingWebServer(this Chassis chassis, Action<IApplicationBuilder> configuration) =>
            chassis.UsingWebServer(new DelegateWebApplicationConfiguration(null, configuration));

        public static Chassis UsingWebServer(this Chassis chassis, Action<IServiceCollection> configuration) =>
            chassis.UsingWebServer(new DelegateWebApplicationConfiguration(configuration));

        public static Chassis UsingWebServer(this Chassis chassis, IConfigureWebApplication configuration)
        {
            return UsingWebServer(chassis, _ => configuration);
        }

        public static Chassis UsingWebServer(
            this Chassis chassis,
            Func<DeploymentConfiguration, IConfigureWebApplication> configuration)
        {
            var bindings = chassis.GetUrlBindings();
            var modules =
                new Func<IEnumerable<Assembly>, IModule[]>(
                    a => chassis.GetModules(chassis.Configuration, a).ToArray());
            var enableConsoleLogging =
                (bool) chassis.Metadata.GetOrDefault(ChassisExtensions.EnableConsoleLogging, true)!;
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (c, a) => new WebServerService(
                    c,
                    new WebStartup(
                        enableConsoleLogging,
                        c,
                        bindings.Distinct().ToArray(),
                        configuration(c),
                        filters as (string, LogLevel)[],
                        modules(a))));
        }

        private static List<string> GetUrlBindings(this Chassis chassis)
        {
            if (!chassis.Metadata.ContainsKey(UrlBindingKey))
            {
                chassis.Metadata.Add(UrlBindingKey, new List<string>());
            }

            return ((List<string>)chassis.Metadata[UrlBindingKey])!;
        }
    }
}
