// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestWebServerChassisExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Blob type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace OpenMedStack.Web.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;
    using global::Autofac.Core;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using OpenMedStack.Autofac;
    using OpenMedStack.Web.Autofac;

    /// <summary>
    /// Defines the extension methods for creating a test web server.
    /// </summary>
    public static class TestWebServerChassisExtensions
    {
        /// <summary>
        /// Creates a <see cref="TestChassis"/> from the given configuration.
        /// </summary>
        /// <param name="chassis">The chassis definition.</param>
        /// <param name="configureWebApplication">The application configuration.</param>
        /// <param name="principal">An optional principal to use in requests.</param>
        /// <returns>A <see cref="TestChassis"/>.</returns>
        public static TestChassis UsingTestWebServer(
            this Chassis chassis,
            IConfigureWebApplication configureWebApplication,
            ClaimsPrincipal? principal = null) =>
            UsingTestWebServer(
                chassis,
                configureWebApplication.ConfigureServices,
                configureWebApplication.ConfigureApplication,
                principal);

        /// <summary>
        /// Creates a <see cref="TestChassis"/> from the given configuration.
        /// </summary>
        /// <param name="chassis">The chassis definition.</param>
        /// <param name="applicationConfiguration">The application configuration.</param>
        /// <param name="principal">An optional principal to use in requests.</param>
        /// <param name="serviceConfiguration">The service configuration.</param>
        /// <returns>A <see cref="TestChassis"/>.</returns>
        public static TestChassis UsingTestWebServer(
            this Chassis chassis,
            Action<IServiceCollection>? serviceConfiguration = null,
            Action<IApplicationBuilder>? applicationConfiguration = null,
            ClaimsPrincipal? principal = null)
        {
            var modules =
                new Func<IEnumerable<Assembly>, IModule[]>(
                    a => chassis.GetModules(chassis.Configuration, a).ToArray());
            var wf = chassis.UsingCustomBuilder(
                (c, a) => new TestWebServerService(
                    c,
                    new TestWebStartup(serviceConfiguration, applicationConfiguration, principal, modules(a))));

            return new TestChassis(wf);
        }
    }
}
