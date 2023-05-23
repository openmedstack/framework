// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudEventHeaders.cs" company="Reimers.dk">
//   Copyright � Reimers.dk
// </copyright>
// <summary>
//   Defines the CloudEventHeaders type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Autofac.MassTransit.CloudEvents;

internal static class CloudEventHeaders
{
    public const string RequestId = "X-OMS-Request-Id";
    public const string ConversationId = "X-OMS-Conversation-Id";
    public const string InitiatorId = "X-OMS-Initiator-Id";
    public const string FaultAddress = "X-OMS-Fault-Address";
    public const string DestinationAddress = "X-OMS-Destination-Address";
    public const string ResponseAddress = "reply-to";
    public const string Expiration = "expiration";
    public const string ContentEncoding = "content_encoding";
}