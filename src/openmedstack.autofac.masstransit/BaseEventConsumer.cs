// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseEventConsumer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the BaseEventConsumer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using global::MassTransit;
using OpenMedStack.Events;

using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
internal class BaseEventConsumer<T> : IConsumer<T>
    where T : BaseEvent
{
    private readonly ILogger<BaseEventConsumer<T>> _logger;
    private readonly IHandleEvents<T>[] _messageHandlers;

    public BaseEventConsumer(IEnumerable<IHandleEvents<T>> messageHandlers, ILogger<BaseEventConsumer<T>> logger)
    {
        _logger = logger;
        _messageHandlers = messageHandlers.ToArray();
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        var headers = new OpenMedStack.MessageHeaders(context.Headers.GetAll());
        var handleTasks = _messageHandlers.Select(handler => handler.Handle(context.Message, headers, context.CancellationToken));
        await Task.WhenAll(handleTasks).ConfigureAwait(false);

        _logger.LogDebug("Consumed {typeofT} with {messageHandlerLength} handler(s).", typeof(T), _messageHandlers.Length);
    }
}