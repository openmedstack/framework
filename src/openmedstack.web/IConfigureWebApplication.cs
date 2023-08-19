namespace OpenMedStack.Web;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Defines the web configuration interface.
/// </summary>
public interface IConfigureWebApplication
{
    /// <summary>
    /// Configures the web application services.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to extend.</param>
    void ConfigureServices(IServiceCollection serviceCollection);

    /// <summary>
    /// Configures the web application.
    /// </summary>
    /// <param name="appBuilder">The <see cref="IApplicationBuilder"/> to configure.</param>
    void ConfigureApplication(IApplicationBuilder appBuilder);
}
