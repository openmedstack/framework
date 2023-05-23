// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventEnvelope.cs" company="Reimers.dk">
//   Copyright � Reimers.dk
// </copyright>
// <summary>
//   Defines the CloudEventEnvelope type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

using System;
using CloudNative.CloudEvents;
using global::MassTransit;

internal class CloudEventEnvelope
{
    /// <inheritdoc />
    public Guid? MessageId { get; set; }

    /// <inheritdoc />
    public Guid? RequestId { get; set; }

    /// <inheritdoc />
    public Guid? CorrelationId { get; set; }

    /// <inheritdoc />
    public Guid? ConversationId { get; set; }

    /// <inheritdoc />
    public Guid? InitiatorId { get; set; }

    /// <inheritdoc />
    public Uri? SourceAddress { get; set; }

    /// <inheritdoc />
    public Uri? DestinationAddress { get; set; }

    /// <inheritdoc />
    public Uri? ResponseAddress { get; set; }

    /// <inheritdoc />
    public Uri? FaultAddress { get; set; }

    /// <inheritdoc />
    public string[] MessageType { get; set; } = Array.Empty<string>();
        
    public CloudEvent CloudEvent { get; set; } = null!;

    /// <inheritdoc />
    public DateTime? ExpirationTime { get; set; }

    /// <inheritdoc />
    public DateTime? SentTime { get; set; }

    /// <inheritdoc />
    public Headers Headers { get; set; } = null!;
}
