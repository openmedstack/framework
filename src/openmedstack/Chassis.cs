// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Chassis.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Chassis class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenMedStack.Commands;
using OpenMedStack.Events;

/// <summary>
/// Defines the Chassis class.
/// </summary>
public static class Chassis
{
    /// <summary>
    /// Creates a <see cref="Chassis{T}"/> from the passed <see cref="DeploymentConfiguration"/>.
    /// </summary>
    /// <param name="manifest">The configuration for the chassis.</param>
    /// <returns>The created chassis.</returns>
    public static Chassis<TConfiguration> From<TConfiguration>(TConfiguration manifest)
        where TConfiguration : DeploymentConfiguration =>
        new(manifest);

    /// <summary>
    /// Creates a <see cref="Chassis{T}"/> from the passed <see cref="IConfiguration"/> list.
    /// </summary>
    /// <param name="configurationBuilder">The function to read the configuration values into the <see cref="TConfiguration"/>.</param>
    /// <param name="configurations">The configuration loaders.</param>
    /// <returns>The created chassis.</returns>
    public static Chassis<TConfiguration> From<TConfiguration>(
        Func<IConfiguration, TConfiguration> configurationBuilder,
        params Action<IConfigurationBuilder>[] configurations)
        where TConfiguration : DeploymentConfiguration
    {
        var builder = new ConfigurationBuilder();
        foreach (var configuration in configurations)
        {
            configuration(builder);
        }

        var root = builder.Build();
        var manifest = configurationBuilder(root);
        return new Chassis<TConfiguration>(manifest);
    }
}

/// <summary>
/// Defines the Chassis class.
/// </summary>
public class Chassis<TConfiguration> : IAsyncDisposable, IObservable<BaseEvent>
    where TConfiguration : DeploymentConfiguration
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly CancellationTokenSource _tokenSource = new();
    private readonly List<Assembly> _assemblies = new();
    private Func<TConfiguration, IEnumerable<Assembly>, IService> _serviceBuilder = BuildService;
    private IService? _service;
    private ChassisInstance? _instance;
    private bool _disposed;

    internal Chassis(TConfiguration manifest)
    {
        Configuration = manifest;
        Metadata = new Dictionary<string, object>();

        AppDomain.CurrentDomain.ProcessExit += ProcessExit;
        Console.CancelKeyPress += CancelKeyPress;
    }

    /// <summary>
    /// Get the metadata definitions for the chassis.
    /// </summary>
    public Dictionary<string, object> Metadata { get; }

    public TConfiguration Configuration { get; }

    /// <summary>
    /// Collects the chassis types from the passed assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The chassis definition.</returns>
    public Chassis<TConfiguration> DefinedIn(params Assembly[] assemblies)
    {
        _assemblies.AddRange(
            _assemblies.Concat(assemblies)
                .DistinctBy((a, b) => string.Equals(a.FullName, b.FullName, StringComparison.OrdinalIgnoreCase)));
        return this;
    }

    /// <summary>
    /// Defines the chassis builder to use.
    /// </summary>
    /// <param name="builder">The function to build the chassis with.</param>
    /// <returns>The built chassis.</returns>
    public Chassis<TConfiguration> UsingCustomBuilder(Func<TConfiguration, IEnumerable<Assembly>, IService> builder)
    {
        _serviceBuilder = builder;
        return this;
    }

    /// <summary>
    /// Creates an instance of an <see cref="IService"/> and starts it.
    /// </summary>
    /// <returns></returns>
    public void Start()
    {
        _service = _serviceBuilder(Configuration, _assemblies);
        var task = _service.Start(_tokenSource.Token);
        _instance = new ChassisInstance(task, _tokenSource.Token);
    }

    /// <summary>
    /// Sends the command to the service service bus end point.
    /// </summary>
    /// <typeparam name="TMsg">The <see cref="Type"/> of <see cref="DomainCommand"/> to send.</typeparam>
    /// <param name="msg">The <see cref="DomainCommand"/> to publish.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the send operation.</param>
    /// <returns>A <see cref="Task"/> encapsulating the send task.</returns>
    public Task<CommandResponse> Send<TMsg>(TMsg msg, CancellationToken cancellationToken = default)
        where TMsg : DomainCommand
    {
        if (_service is null)
        {
            throw new InvalidOperationException(Strings.ChassisNotStarted);
        }

        return _service.Send(msg, cancellationToken);
    }

    /// <summary>
    /// Publishes the event to the service service bus end point.
    /// </summary>
    /// <typeparam name="TMsg">The <see cref="Type"/> of <see cref="BaseEvent"/> to publish.</typeparam>
    /// <param name="msg">The <see cref="BaseEvent"/> to publish.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the publish operation.</param>
    /// <returns>A <see cref="Task"/> encapsulating the publish task.</returns>
    public Task Publish<TMsg>(TMsg msg, CancellationToken cancellationToken = default) where TMsg : BaseEvent
    {
        if (_service is null)
        {
            throw new InvalidOperationException(Strings.ChassisNotStarted);
        }

        return _service.Publish(msg, cancellationToken);
    }

    public T Resolve<T>() where T : class
    {
        if (_service is null)
        {
            throw new InvalidOperationException(Strings.ChassisNotStarted);
        }

        return _service.Resolve<T>();
    }

    private static IService BuildService(DeploymentConfiguration manifest, IEnumerable<Assembly> assemblies) =>
        throw new MustImplementException();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }
        finally
        {
            _semaphore.Release();
        }

        _tokenSource.Cancel();
        _assemblies.Clear();
        _service.TryDispose();
        foreach (var value in Metadata.Values)
        {
            value.TryDispose();
        }

        Metadata.Clear();
        AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
        Console.CancelKeyPress -= CancelKeyPress;
        if (_instance != null)
        {
            await _instance.DisposeAsync().ConfigureAwait(false);
        }

        _tokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    private void CancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs)
    {
        _tokenSource.Cancel();
        // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
        eventArgs.Cancel = true;
    }

    private void ProcessExit(object? sender, EventArgs eventArgs)
    {
        _tokenSource.Cancel();
        // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
        // Suppress that since we shut down gracefully. https://github.com/dotnet/aspnetcore/issues/6526
        Environment.ExitCode = 0;
    }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<BaseEvent> observer)
    {
        if (_service is null)
        {
            throw new InvalidOperationException(Strings.ChassisNotStarted);
        }

        return _service.Subscribe(observer);
    }

    private class ChassisInstance : IAsyncDisposable
    {
        private readonly ManualResetEventSlim _waitHandle = new(false);
        private readonly Task _service;

        public ChassisInstance(Task service, CancellationToken cancellationToken)
        {
            _service = Task.Run(
                async () =>
                {
                    try
                    {
                        await service.ConfigureAwait(false);
                        _waitHandle.Wait(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        _waitHandle.Set();
                    }
                },
                cancellationToken);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _service.ConfigureAwait(false);
            _service.Dispose();
        }
    }

    private class MustImplementException : Exception
    {
        public MustImplementException() : base(
            Strings.ChassisBuilderNotConfigured)
        {
        }
    }
}
