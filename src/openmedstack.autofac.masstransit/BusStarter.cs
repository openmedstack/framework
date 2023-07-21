// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusStarter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the BusStarter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit;

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

    public async Task Setup(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message bus starting.");
        await _control.StartAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Message bus started.");
    }

    public async Task Shutdown(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message bus shutting down.");
        await _control.StopAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Message bus shut down.");
    }

    public void Dispose()
    {
    }
}
