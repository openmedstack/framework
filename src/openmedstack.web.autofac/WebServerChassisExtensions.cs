namespace OpenMedStack.Web.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Autofac;

    public static class ChassisExtensions
    {
        private const string UrlBindingKey = "WebUrlBindings";
        private const string LogFilters = "LogFilters";

        public static Chassis<TConfiguration> BindToUrls<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            params string[] urls)
            where TConfiguration : WebDeploymentConfiguration
        {
            chassis.GetUrlBindings().AddRange(urls);
            return chassis;
        }

        public static Chassis<TConfiguration> UsingWebServer<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            Action<IApplicationBuilder> configuration)
            where TConfiguration : WebDeploymentConfiguration =>
            chassis.UsingWebServer(new DelegateWebApplicationConfiguration(null, configuration));

        public static Chassis<TConfiguration> UsingWebServer<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            Action<IServiceCollection> configuration)
            where TConfiguration : WebDeploymentConfiguration =>
            chassis.UsingWebServer(new DelegateWebApplicationConfiguration(configuration));

        public static Chassis<TConfiguration> UsingWebServer<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            IConfigureWebApplication configuration)
            where TConfiguration : WebDeploymentConfiguration
        {
            return UsingWebServer(chassis, _ => configuration);
        }

        public static Chassis<TConfiguration> UsingWebServer<TConfiguration>(
            this Chassis<TConfiguration> chassis,
            Func<TConfiguration, IConfigureWebApplication> configuration)
            where TConfiguration : WebDeploymentConfiguration
        {
            var bindings = chassis.GetUrlBindings();
            var enableConsoleLogging = (bool)chassis.Metadata.GetOrDefault(
                OpenMedStack.Autofac.ChassisExtensions.EnableConsoleLogging,
                true);
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (c, a) => new WebServerService<TConfiguration>(
                    c,
                    new WebStartup<TConfiguration>(
                        enableConsoleLogging,
                        c,
                        bindings.Distinct().ToArray(),
                        configuration(c),
                        filters as (string, LogLevel)[],
                        chassis.GetModules(chassis.Configuration, a).ToArray())));
        }

        private static List<string> GetUrlBindings<TConfiguration>(this Chassis<TConfiguration> chassis)
            where TConfiguration : WebDeploymentConfiguration
        {
            if (!chassis.Metadata.ContainsKey(UrlBindingKey))
            {
                chassis.Metadata.Add(UrlBindingKey, new List<string>());
            }

            return ((List<string>)chassis.Metadata[UrlBindingKey]);
        }
    }
}
