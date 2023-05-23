namespace OpenMedStack.Web.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using global::Autofac;
    using global::Autofac.Core;
    using global::Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Autofac;
    using OpenMedStack.Events;

    internal class WebStartup<TConfiguration> : IStartup
        where TConfiguration : WebDeploymentConfiguration
    {
        private readonly bool _enableConsoleLogging;
        private readonly TConfiguration _deploymentConfiguration;
        private readonly IConfigureWebApplication _webappConfiguration;
        private readonly (string, LogLevel)[]? _filters;
        private readonly IModule[] _modules;
        private IContainer _container = null!;

        public WebStartup(
            bool enableConsoleLogging,
            TConfiguration deploymentConfiguration,
            IEnumerable<string> urlBindings,
            IConfigureWebApplication builder,
            (string, LogLevel)[]? filters = null,
            params IModule[] modules)
        {
            UrlBindings = urlBindings.ToArray();
            _enableConsoleLogging = enableConsoleLogging;
            _deploymentConfiguration = deploymentConfiguration;
            _webappConfiguration = builder;
            _filters = filters;
            _modules = modules;
        }

        public string[] UrlBindings { get; }

        public long? MaxRequestSize { get; } = 1024 * 1024;

        public long? MaxConcurrentConnections { get; } = 10_000;

        public TimeSpan KeepAliveTimeout { get; } = TimeSpan.FromHours(1);

        /// <inheritdoc />
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _webappConfiguration.ConfigureServices(services);
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConsoleLogModule(_enableConsoleLogging, _filters));
            builder.RegisterModule(new ValidationModule());
            builder.RegisterInstance(_deploymentConfiguration)
                .As<TConfiguration>()
                .As<DeploymentConfiguration>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
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
            _webappConfiguration.ConfigureApplication(app);
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            // ReSharper disable once AsyncVoidLambda
            lifetime.ApplicationStopped.Register(
                async () => { await _container.DisposeAsync().ConfigureAwait(false); });
        }
    }
}
