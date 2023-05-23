namespace OpenMedStack.Web.Autofac;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class DelegateWebApplicationConfiguration : IConfigureWebApplication
{
    private readonly Action<IServiceCollection> _serviceConfiguration;
    private readonly Action<IApplicationBuilder> _applicationConfiguration;

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
