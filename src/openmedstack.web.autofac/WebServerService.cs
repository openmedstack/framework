namespace OpenMedStack.Web.Autofac;

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
internal class WebServerService<TConfiguration> : IService
    where TConfiguration : WebDeploymentConfiguration
{
    private readonly TConfiguration _manifest;
    private readonly ISubject<BaseEvent> _subject = new Subject<BaseEvent>();
    private readonly IWebHost _container;
    private readonly IPublishEvents _eventBus;
    private readonly IRouteCommands _commandBus;
    private readonly string _urlBinding;
    private readonly ILogger<WebServerService<TConfiguration>> _logger;

    public WebServerService(TConfiguration manifest, WebStartup<TConfiguration> startup)
    {
        _manifest = manifest;
        _urlBinding = string.Join(", ", startup.UrlBindings);
        _container = new WebHostBuilder().UseSetting(WebHostDefaults.ApplicationKey, _manifest.Name)
            .UseContentRoot(_manifest.ContentRoot ?? Directory.GetCurrentDirectory())
            .UseKestrel(
                options =>
                {
                    options.AddServerHeader = false;
                    options.Limits.MaxRequestHeadersTotalSize = (int)Math.Pow(2, 16);
                    options.Limits.MaxRequestBodySize = startup.MaxRequestSize;
                    options.Limits.KeepAliveTimeout = startup.KeepAliveTimeout;
                    options.Limits.MaxConcurrentConnections = startup.MaxConcurrentConnections;
                })
            .ConfigureServices(
                x =>
                {
                    x.AddSingleton(_manifest);
                    x.AddSingleton<DeploymentConfiguration>(_manifest);
                    x.AddSingleton<WebDeploymentConfiguration>(_manifest);
                    x.AddSingleton(_subject);
                    x.AddSingleton<IStartup>(startup);
                })
            .UseUrls(startup.UrlBindings).Build();
        _logger = _container.Services.GetRequiredService<ILogger<WebServerService<TConfiguration>>>();
        _eventBus = _container.Services.GetRequiredService<IPublishEvents>();
        _commandBus = _container.Services.GetRequiredService<IRouteCommands>();
    }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<BaseEvent> observer) => _subject.SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        var bootstrappers = _container.Services.GetServices<IBootstrapSystem>();
        foreach (var bootstrapper in bootstrappers.OrderByDescending(x => x.Order))
        {
            using var tokenSource = new CancellationTokenSource(_manifest.Timeout);
            await bootstrapper.Shutdown(tokenSource.Token).ConfigureAwait(false);
        }

        using var cancellationTokenSource = new CancellationTokenSource(_manifest.Timeout);
        await _container.StopAsync(cancellationTokenSource.Token).ConfigureAwait(false);
        _subject.TryDispose();
        _container.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual async Task Start(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting web server...");
        var startupValidations = _container.Services.GetServices<IValidateStartup>();
        var validationTasks = startupValidations.Select(x => x.Validate());
        var startupErrors = (await Task.WhenAll(validationTasks).ConfigureAwait(false)).NonNulls().ToArray();
        if (startupErrors.Length > 0)
        {
            throw new AggregateException("Startup validation failed", startupErrors);
        }
        var bootstrappers = _container.Services.GetServices<IBootstrapSystem>();
        foreach (var bootstrapper in bootstrappers.OrderBy(x => x.Order))
        {
            await bootstrapper.Setup(cancellationToken).ConfigureAwait(false);
        }

        await _container.StartAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Web server bound to: {Url}", _urlBinding);
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
