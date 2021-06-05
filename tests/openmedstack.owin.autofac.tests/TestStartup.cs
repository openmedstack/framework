namespace OpenMedStack.Web.Autofac.Tests
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;
    using OpenMedStack.Web.Autofac;

    internal class TestStartup : IConfigureWebApplication
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
        }

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
            app.Run(
                async ctx =>
                {
                    if (ctx.Request.Path == "/commands")
                    {
                        var commandBus = ctx.RequestServices.GetRequiredService<IRouteCommands>();
                        var response = await commandBus.Send(new TestCommand()).ConfigureAwait(false);
                        await ctx.Response.WriteAsync(
                                string.IsNullOrWhiteSpace(response.FaultMessage) ? "Success" : response.FaultMessage)
                            .ConfigureAwait(false);
                        return;
                    }
                    var bus = ctx.RequestServices.GetRequiredService<IPublishEvents>();
                    await bus.Publish(new TestDomainEvent(Guid.NewGuid().ToString(), 0)).ConfigureAwait(false);
                });
        }
    }
}
