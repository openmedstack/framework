namespace OpenMedStack.Web.Autofac;

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMedStack.Autofac;

/// <summary>
/// Defines the extensions for the web server chassis.
/// </summary>
public static class ChassisExtensions
{
    private const string LogFilters = "LogFilters";

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
                    c.Urls,
                    configuration(c),
                    filters as (string, LogLevel)[],
                    chassis.GetModules(chassis.Configuration, a).ToArray())));
    }
}
