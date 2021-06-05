// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MassTransitEventSubscriber.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the MassTransitEventSubscriber type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit
{
    using System;
    using global::MassTransit;
    using OpenMedStack.Events;

    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    internal class MassTransitEventSubscriber<T> : ISubscribeEvents<T>
        where T : BaseEvent
    {
        private readonly IBus _bus;
        private readonly Func<ILogger<BaseEventConsumer<T>>> _loggerFunc;

        public MassTransitEventSubscriber(IBus bus, Func<ILogger<BaseEventConsumer<T>>> loggerFunc)
        {
            _bus = bus;
            _loggerFunc = loggerFunc;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        public Task<IDisposable> Subscribe(IHandleEvents<T> handler) =>
            Task.FromResult<IDisposable>(_bus.ConnectConsumer(() => new BaseEventConsumer<T>(new[] { handler }, _loggerFunc())));
    }
}
