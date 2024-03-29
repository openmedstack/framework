﻿namespace OpenMedStack.Web.Testing;

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMedStack;
using OpenMedStack.Commands;
using OpenMedStack.Events;
using OpenMedStack.Web.Autofac;

/// <summary>
/// Defines the Autofac based implementation of <see cref="IService"/>.
/// </summary>
internal class TestWebServerService<TConfiguration> : ITestWebService
    where TConfiguration : WebDeploymentConfiguration
{
    private readonly TConfiguration _manifest;
    private ISubject<BaseEvent> _subject = null!;
    private readonly TestServer _container;
    private IPublishEvents _eventBus = null!;
    private IRouteCommands _commandBus = null!;

    public TestWebServerService(TConfiguration manifest, TestWebStartup<TConfiguration> startup)
    {
        _manifest = manifest;
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(x => x.AddSingleton<IStartup>(startup))
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseSetting(WebHostDefaults.ApplicationKey, _manifest.Name);

        _container = new TestServer(hostBuilder); //TestWebServer(hostBuilder);
    }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<BaseEvent> observer) => _subject.SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        var bootstrappers = _container.Host.Services.GetServices<IBootstrapSystem>();
        foreach (var bootstrapper in bootstrappers.OrderByDescending(x => x.Order))
        {
            using var tokenSource = new CancellationTokenSource(_manifest.Timeout);
            await bootstrapper.Shutdown(tokenSource.Token).ConfigureAwait(false);
        }

        await _container.Host.StopAsync(_manifest.Timeout).ConfigureAwait(false);

        _subject.TryDispose();
        _container.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual async Task Start(CancellationToken cancellationToken = default)
    {
        var bootstrappers = _container.Host.Services.GetServices<IBootstrapSystem>();
        var logger = _container.Host.Services.GetRequiredService<ILogger<TestWebServerService<TConfiguration>>>();
        foreach (var bootstrapper in bootstrappers.OrderBy(x => x.Order))
        {
            await bootstrapper.Setup(cancellationToken).ConfigureAwait(false);
        }

        try
        {
            await _container.Host.StartAsync(cancellationToken).ConfigureAwait(false);
            _subject = _container.Host.Services.GetService<ISubject<BaseEvent>>()
             ?? throw new Exception($"Could not resolve {nameof(ISubject<BaseEvent>)}");
            _eventBus = _container.Host.Services.GetService<IPublishEvents>()
             ?? throw new Exception($"Could not resolve {nameof(IPublishEvents)}");
            _commandBus = _container.Host.Services.GetService<IRouteCommands>()
             ?? throw new Exception($"Could not resolve {nameof(IRouteCommands)}");
            logger.LogInformation("Web server bound to: {address}", _container.BaseAddress);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
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
    public T Resolve<T>()
        where T : class
    {
        if (typeof(T) == typeof(HttpClient))
        {
            return (_container.CreateClient() as T)!;
        }

        if (typeof(HttpMessageHandler).IsAssignableFrom(typeof(T)))
        {
            return (_container.CreateHandler() as T)!;
        }
        return _container.Host.Services.GetRequiredService<T>();
    }

    /// <inheritdoc />
    public HttpClient CreateClient() => _container.CreateClient();
}