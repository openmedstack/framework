// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MassTransitEventBus.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the MassTransitEventBus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenMedStack.Events;

using global::MassTransit;
using Microsoft.Extensions.Logging;

internal class MassTransitEventBus : IPublishEvents
{
    private readonly IPublishEndpoint _serviceBus;
    private readonly ILogger<MassTransitEventBus> _logger;

    public MassTransitEventBus(IPublishEndpoint serviceBus, ILogger<MassTransitEventBus> logger)
    {
        _serviceBus = serviceBus;
        _logger = logger;
    }

    public async Task Publish<T>(T message, IDictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
        where T : BaseEvent
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _serviceBus.Publish(message, new EventHeaderPipe(headers), cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("{type} published", typeof(T).Name);
    }
}