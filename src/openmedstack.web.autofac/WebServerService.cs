namespace OpenMedStack.Web.Autofac
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;
    using OpenMedStack.Startup;

    /// <summary>
    /// Defines the Autofac based implementation of <see cref="IService"/>.
    /// </summary>
    internal class WebServerService : IService
    {
        private readonly DeploymentConfiguration _manifest;
        private readonly ISubject<BaseEvent> _subject = new Subject<BaseEvent>();
        private readonly IWebHost _container;
        private readonly IPublishEvents _eventBus;
        private readonly IRouteCommands _commandBus;
        private readonly string _urlBinding;

        public WebServerService(DeploymentConfiguration manifest, WebStartup startup)
        {
            _manifest = manifest;
            var urlBindings = startup.UrlBindings.ToArray();
            _urlBinding = string.Join(", ", urlBindings);
            _container = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Limits.MaxRequestHeadersTotalSize = (int)Math.Pow(2, 16);
                    options.Limits.MaxRequestBodySize = startup.MaxRequestSize;
                    options.Limits.KeepAliveTimeout = startup.KeepAliveTimeout;
                    options.Limits.MaxConcurrentConnections = startup.MaxConcurrentConnections;
                })
                .ConfigureServices(x =>
                {
                    x.AddSingleton<ISubject<BaseEvent>>(_subject);
                    x.AddSingleton<IStartup>(startup);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSetting(WebHostDefaults.ApplicationKey, _manifest.Name)
                .UseUrls(urlBindings)
                .Build();
            _eventBus = _container.Services.GetRequiredService<IPublishEvents>();
            _commandBus = _container.Services.GetRequiredService<IRouteCommands>();
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<BaseEvent> observer) => _subject.SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer);

        /// <inheritdoc />
        public void Dispose()
        {
            var bootstrappers = _container.Services.GetServices<IBootstrapSystem>();
            foreach (var bootstrapper in bootstrappers.OrderByDescending(x => x.Order))
            {
                bootstrapper.Shutdown(CancellationToken.None).Wait(_manifest.Timeout);
            }

            _container.StopAsync(TimeSpan.FromMinutes(3)).Wait();
            _subject.TryDispose();
            _container.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual async Task Start(CancellationToken cancellationToken = default)
        {
            var startupValidations = _container.Services.GetServices<IValidateStartup>();
            var validationTasks = startupValidations.Select(x => x.Validate());
            var startupErrors = (await Task.WhenAll(validationTasks).ConfigureAwait(false)).NonNulls().ToArray();
            if (startupErrors.Length > 0)
            {
                throw new AggregateException("Startup validation failed", startupErrors);
            }
            var bootstrappers = _container.Services.GetServices<IBootstrapSystem>();
            var logger = _container.Services.GetRequiredService<ILogger<WebServerService>>();
            foreach (var bootstrapper in bootstrappers.OrderBy(x => x.Order))
            {
                await bootstrapper.Setup(cancellationToken).ConfigureAwait(false);
            }

            await _container.StartAsync(cancellationToken);
            logger.LogInformation("Web server bound to: {url}", _urlBinding);
        }

        /// <inheritdoc />
        public Task<CommandResponse> Send<T>(
            T msg,
            CancellationToken cancellationToken = new CancellationToken())
            where T : DomainCommand => _commandBus.Send(msg, null, cancellationToken);

        /// <inheritdoc />
        public Task Publish<T>(T msg, CancellationToken cancellationToken)
            where T : BaseEvent => _eventBus.Publish(msg, cancellationToken: cancellationToken);

        /// <inheritdoc />
        public T Resolve<T>() where T : class => _container.Services.GetRequiredService<T>();
    }
}
