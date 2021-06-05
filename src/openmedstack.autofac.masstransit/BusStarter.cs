// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusStarter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the BusStarter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::MassTransit;

    using Microsoft.Extensions.Logging;

    internal class BusStarter : IBootstrapSystem, IDisposable
    {
        private readonly IBusControl _control;
        private readonly ILogger<BusStarter> _logger;

        public BusStarter(IBusControl control, ILogger<BusStarter> logger)
        {
            _control = control;
            _logger = logger;
        }

        public uint Order => uint.MinValue;

        public Task Setup(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message bus starting.");
            return _control.StartAsync(cancellationToken);
        }

        public Task Shutdown(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message bus shutting down.");
            return _control.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}