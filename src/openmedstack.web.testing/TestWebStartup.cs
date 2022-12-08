namespace OpenMedStack.Web.Testing
{
    using System;
    using System.Reactive.Subjects;
    using System.Security.Claims;
    using global::Autofac;
    using global::Autofac.Core;
    using global::Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using OpenMedStack.Events;
    using OpenMedStack.Web.Autofac;

    internal class TestWebStartup : IStartup
    {
        private readonly Action<IServiceCollection>? _webappConfiguration;
        private readonly Action<IApplicationBuilder>? _configureApplication;
        private readonly ClaimsPrincipal? _principal;
        private readonly IModule[] _modules;
        private IContainer _container = null!;

        public TestWebStartup(IConfigureWebApplication configureApplication, ClaimsPrincipal? principal = null, params IModule[] modules)
            : this(configureApplication.ConfigureServices, configureApplication.ConfigureApplication, principal, modules)
        {
        }

        public TestWebStartup(Action<IServiceCollection>? builder = null, Action<IApplicationBuilder>? configureApplication = null, ClaimsPrincipal? principal = null, params IModule[] modules)
        {
            _webappConfiguration = builder;
            _configureApplication = configureApplication;
            _principal = principal;
            _modules = modules;
        }

        /// <inheritdoc />
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _webappConfiguration?.Invoke(services);
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterInstance(new Subject<BaseEvent>()).AsImplementedInterfaces().SingleInstance();
            foreach (var module in _modules)
            {
                builder.RegisterModule(module);
            }

            _container = builder.Build();
            return new AutofacServiceProvider(_container);
        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app)
        {
            app.Use(
                (ctx, next) =>
                {
                    if (_principal != null)
                    {
                        ctx.User = _principal;
                    }
                    return next();
                });
            _configureApplication?.Invoke(app);
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopped.Register(_container.Dispose);
        }
    }
}