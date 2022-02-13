// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Chassis.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Chassis class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;

    /// <summary>
    /// Defines the Chassis class.
    /// </summary>
    public class Chassis : IDisposable, IObservable<BaseEvent>
    {
        private readonly ManualResetEventSlim _waitHandle = new(false);
        private List<Assembly> _assemblies = new();
        private Func<DeploymentConfiguration, IEnumerable<Assembly>, IService> _serviceBuilder = BuildService;
        private IService? _service;

        private Chassis(DeploymentConfiguration manifest)
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

        public DeploymentConfiguration Configuration { get; }

        /// <summary>
        /// Creates a <see cref="Chassis"/> from the passed <see cref="DeploymentConfiguration"/>.
        /// </summary>
        /// <param name="manifest">The configuration for the chassis.</param>
        /// <returns>The created chassis.</returns>
        public static Chassis From(DeploymentConfiguration manifest) => new(manifest);

        /// <summary>
        /// Collects the chassis types from the passed assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <returns>The chassis definition.</returns>
        public Chassis DefinedIn(params Assembly[] assemblies)
        {
            _assemblies = _assemblies.Concat(assemblies)
                .DistinctBy((a, b) => string.Equals(a.FullName, b.FullName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return this;
        }

        /// <summary>
        /// Defines the chassis builder to use.
        /// </summary>
        /// <param name="builder">The function to build the chassis with.</param>
        /// <returns>The built chassis.</returns>
        public Chassis UsingCustomBuilder(Func<DeploymentConfiguration, IEnumerable<Assembly>, IService> builder)
        {
            _serviceBuilder = builder;
            return this;
        }

        /// <summary>
        /// Creates an instance of an <see cref="IService"/> and starts it.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Start(CancellationToken cancellationToken = default)
        {
            _service = _serviceBuilder(Configuration, _assemblies);
            var task = _service.Start(cancellationToken);
            return Task.Run(async () =>
                {
                    await task.ConfigureAwait(false);
                    _waitHandle.Wait(cancellationToken);
                }, cancellationToken);
        }

        /// <summary>
        /// Sends the command to the service service bus end point.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="DomainCommand"/> to send.</typeparam>
        /// <param name="msg">The <see cref="DomainCommand"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the send operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the send task.</returns>
        public Task<CommandResponse> Send<T>(T msg, CancellationToken cancellationToken = default)
            where T : DomainCommand
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
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="BaseEvent"/> to publish.</typeparam>
        /// <param name="msg">The <see cref="BaseEvent"/> to publish.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the publish operation.</param>
        /// <returns>A <see cref="Task"/> encapsulating the publish task.</returns>
        public Task Publish<T>(T msg, CancellationToken cancellationToken = default) where T : BaseEvent
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

        private static IService BuildService(DeploymentConfiguration manifest, IEnumerable<Assembly> assemblies) => throw new NotImplementedException(Strings.ChassisBuilderNotConfigured);

        /// <inheritdoc />
        public void Dispose()
        {
            _waitHandle.Set();
            _assemblies.Clear();
            _service.TryDispose();
            foreach (var value in Metadata.Values)
            {
                value.TryDispose();
            }

            Metadata.Clear();
            AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
            Console.CancelKeyPress -= CancelKeyPress;
            _waitHandle.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs)
        {
            // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
            eventArgs.Cancel = true;
            _waitHandle.Set();
        }

        private void ProcessExit(object? sender, EventArgs eventArgs)
        {
            // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
            // Suppress that since we shut down gracefully. https://github.com/dotnet/aspnetcore/issues/6526
            Environment.ExitCode = 0;
            _waitHandle.Set();
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
    }
}
