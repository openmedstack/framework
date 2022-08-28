// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacService.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the Autofac based implementation of <see cref="IService" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Autofac;
    using global::Autofac.Core;
    using Microsoft.Extensions.Logging;
    using OpenMedStack.Commands;
    using OpenMedStack.Events;
    using OpenMedStack.Startup;

    /// <summary>
    /// Defines the Autofac based implementation of <see cref="IService"/>.
    /// </summary>
    public class AutofacService : IService
    {
        private readonly DeploymentConfiguration _configuration;
        private readonly Subject<BaseEvent> _subject = new();
        private readonly IContainer _container;
        private readonly IPublishEvents _eventBus;
        private readonly IRouteCommands _commandBus;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacService"/> class.
        /// </summary>
        /// <param name="configuration">The deployment configuration</param>
        /// <param name="enableConsoleLogging">Register a console logger.</param>
        /// <param name="filters">The set of log level filters to apply.</param>
        /// <param name="modules">The <see cref="IModule"/> to use for configuration.</param>
        public AutofacService(DeploymentConfiguration configuration, bool enableConsoleLogging = true, (string, LogLevel)[]? filters = null, params IModule[] modules)
        {
            _configuration = configuration;
            var builder = new ContainerBuilder();
            builder.RegisterInstance(configuration)
                .As<DeploymentConfiguration>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterModule(new ConsoleLogModule(enableConsoleLogging, filters ?? Array.Empty<(string, LogLevel)>()));
            builder.RegisterModule(new ValidationModule());
            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }

            builder.RegisterInstance(_subject).As<ISubject<BaseEvent>>();

            _container = builder.Build();

            _commandBus = _container.Resolve<IRouteCommands>();
            _eventBus = _container.Resolve<IPublishEvents>();
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<BaseEvent> observer) =>
            _subject.SubscribeOn(TaskPoolScheduler.Default).Subscribe(observer);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual async Task Start(CancellationToken cancellationToken)
        {
            var startupValidations = _container.Resolve<IEnumerable<IValidateStartup>>();
            var validationTasks = startupValidations.Select(x => x.Validate());
            var startupErrors = (await Task.WhenAll(validationTasks).ConfigureAwait(false)).NonNulls().ToArray();
            if (startupErrors.Length > 0)
            {
                throw new AggregateException("Startup validation failed", startupErrors);
            }
            var bootstrappers = _container.Resolve<IEnumerable<IBootstrapSystem>>();
            foreach (var bootstrapper in bootstrappers.OrderBy(x => x.Order))
            {
                await bootstrapper.Setup(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public Task<CommandResponse> Send<T>(T msg, CancellationToken cancellationToken = new CancellationToken())
            where T : DomainCommand => _commandBus.Send(msg, null, cancellationToken);

        /// <inheritdoc />
        public Task Publish<T>(T msg, CancellationToken cancellationToken) where T : BaseEvent =>
            _eventBus.Publish(msg, cancellationToken: cancellationToken);

        /// <inheritdoc />
        public T Resolve<T>() where T : class => _container.Resolve<T>();

        /// <summary>
        /// Disposes managed and unmanaged resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> if called from Disposing() method. <c>false</c> if called from finalizer.</param>
        protected virtual async ValueTask Dispose(bool isDisposing)
        {
            if (_isDisposed || !isDisposing)
            {
                return;
            }

            _isDisposed = true;
            var bootstrappers = _container.Resolve<IEnumerable<IBootstrapSystem>>();
            foreach (var bootstrapper in bootstrappers.GroupBy(x => x.Order).OrderByDescending(x => x.Key))
            {
                using var tokenSource = new CancellationTokenSource(_configuration.Timeout);
                await Task.WhenAll(bootstrapper.Select(x => x.Shutdown(tokenSource.Token)).ToArray());
            }

            _subject.Dispose();
            _container.Dispose();
        }
    }
}
