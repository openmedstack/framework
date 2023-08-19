namespace OpenMedStack.Web.Autofac;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Defines the web application configuration using delegates.
/// </summary>
public class DelegateWebApplicationConfiguration : IConfigureWebApplication
{
    private readonly Action<IServiceCollection> _serviceConfiguration;
    private readonly Action<IApplicationBuilder> _applicationConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateWebApplicationConfiguration"/> class.
    /// </summary>
    /// <param name="serviceConfiguration">The service configuration delegate.</param>
    /// <param name="applicationConfiguration">The application configuration delegate.</param>
    public DelegateWebApplicationConfiguration(
        Action<IServiceCollection>? serviceConfiguration = null,
        Action<IApplicationBuilder>? applicationConfiguration = null)
    {
        _serviceConfiguration = serviceConfiguration ?? (_ => { });
        _applicationConfiguration = applicationConfiguration ?? (_ => { });
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection serviceCollection) => _serviceConfiguration(serviceCollection);

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder appBuilder) => _applicationConfiguration(appBuilder);
}
