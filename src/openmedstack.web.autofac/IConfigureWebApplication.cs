namespace OpenMedStack.Web.Autofac
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public interface IConfigureWebApplication
    {
        void ConfigureServices(IServiceCollection serviceCollection);

        void ConfigureApplication(IApplicationBuilder appBuilder);
    }
}