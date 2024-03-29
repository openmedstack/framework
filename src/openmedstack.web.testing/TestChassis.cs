﻿namespace OpenMedStack.Web.Testing;

using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenMedStack.Commands;
using OpenMedStack.Events;
using OpenMedStack.Web.Autofac;

/// <summary>
/// Defines the chassis for tests
/// </summary>
public class TestChassis<TConfiguration> : IAsyncDisposable
    where TConfiguration : WebDeploymentConfiguration
{
    private readonly Chassis<TConfiguration> _chassis;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestChassis{T}"/> class.
    /// </summary>
    /// <param name="chassis"></param>
    public TestChassis(Chassis<TConfiguration> chassis)
    {
        _chassis = chassis;
    }

    /// <summary>
    /// Starts the chassis.
    /// </summary>
    /// <returns>The started <see cref="ITestWebService"/> as an asynchronous operation.</returns>
    public void Start() => _chassis.Start();

    public HttpClient CreateClient() => _chassis.Resolve<HttpClient>();

    public T Resolve<T>() where T : class => _chassis.Resolve<T>();

    public Task Publish<T>(T message, CancellationToken cancellationToken) where T : BaseEvent =>
        _chassis.Publish(message, cancellationToken);

    public Task<CommandResponse> Send<T>(T message, CancellationToken cancellationToken) where T : DomainCommand =>
        _chassis.Send(message, cancellationToken);

    public IDisposable Subscribe(IObserver<BaseEvent> observer) => _chassis.Subscribe(observer);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _chassis.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public IDisposable Subscribe(Action<BaseEvent> func) =>
        _chassis.SubscribeOn(TaskPoolScheduler.Default).Subscribe(func);
}
