// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusSpy.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the BusSpy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit;

using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using global::MassTransit;
using Microsoft.Extensions.Logging;
using OpenMedStack.Events;

internal class BusSpy : IPublishObserver
{
    private readonly ISubject<BaseEvent> _subject;
    private readonly ILogger<BusSpy> _logger;

    public BusSpy(ISubject<BaseEvent> subject, ILogger<BusSpy> logger)
    {
        _subject = subject;
        _logger = logger;
    }

    private Task OnError<T>(Exception exception) where T : class
    {
        using (_logger.BeginScope(exception))
        {
            _logger.LogError("Error sending payload of type " + typeof(T).Name);
            _logger.LogError(exception.Message);
            _logger.LogError(exception.StackTrace);
        }

        _subject.OnError(exception);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task PrePublish<T>(PublishContext<T> context) where T : class => Task.CompletedTask;

    /// <inheritdoc />
    public Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        _logger.LogDebug("Message published with payload of type: " + typeof(T).Name);

        if (context.Message is BaseEvent message)
        {
            _subject.OnNext(message);
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class => OnError<T>(exception);
}