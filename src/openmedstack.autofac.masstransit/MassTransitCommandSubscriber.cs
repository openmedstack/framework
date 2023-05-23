// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MassTransitCommandSubscriber.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the MassTransitCommandSubscriber type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using MassTransit;
using OpenMedStack.Commands;

namespace OpenMedStack.Autofac.MassTransit;

using System.Threading.Tasks;

internal class MassTransitCommandSubscriber<T> : ISubscribeCommands<T>
    where T : DomainCommand
{
    private readonly IBus _bus;

    public MassTransitCommandSubscriber(IBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
    }

    public Task<IDisposable> Subscribe(IHandleCommands<T> handler)
    {
        var handle = _bus.ConnectConsumer(() => new CommandConsumer<T>(handler));
        return Task.FromResult<IDisposable>(handle);
    }
}
