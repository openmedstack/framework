namespace OpenMedStack.Web.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            Func<WebDeploymentConfiguration, IConfigureWebApplication> configuration)
        {
            var bindings = chassis.GetUrlBindings();
            var enableConsoleLogging =
                (bool)chassis.Metadata.GetOrDefault(ChassisExtensions.EnableConsoleLogging, true);
            chassis.Metadata.TryGetValue(LogFilters, out var filters);
            return chassis.UsingCustomBuilder(
                (c, a) =>
                {
                    var webDeploymentConfiguration = (WebDeploymentConfiguration)c;
                    return new WebServerService(
                        webDeploymentConfiguration,
                        new WebStartup(
                            enableConsoleLogging,
                            c,
                            bindings.Distinct().ToArray(),
                            configuration(webDeploymentConfiguration),
                            filters as (string, LogLevel)[],
                            chassis.GetModules(chassis.Configuration, a).ToArray()));
                });
        }

        private static List<string> GetUrlBindings(this Chassis chassis)
        {
            if (!chassis.Metadata.ContainsKey(UrlBindingKey))
            {
                chassis.Metadata.Add(UrlBindingKey, new List<string>());
            }

            return ((List<string>)chassis.Metadata[UrlBindingKey]);
        }
    }
}
