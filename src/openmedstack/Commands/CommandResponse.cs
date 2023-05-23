// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandResponse.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the command response class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.Commands;

/// <summary>
/// Defines the command response class.
/// </summary>
//[Topic("CommandResponse")]
public class CommandResponse : ICorrelate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandResponse"/> class.
    /// </summary>
    /// <param name="targetAggregate">
    /// The target aggregate.
    /// </param>
    /// <param name="version">
    /// The version.
    /// </param>
    /// <param name="faultMessage">
    /// The fault message.
    /// </param>
    /// <param name="correlationId">
    /// The correlation id.
    /// </param>
    public CommandResponse(string targetAggregate, int version, string? faultMessage, string? correlationId)
    {
        TargetAggregate = targetAggregate;
        Version = version;
        FaultMessage = faultMessage;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Gets the fault message.
    /// </summary>
    public string? FaultMessage { get; }

    /// <summary>
    /// Gets the id of the receiving aggregate.
    /// </summary>
    public string TargetAggregate { get; }

    /// <summary>
    /// Gets the version of the target aggregate.
    /// </summary>
    public int Version { get; }

    /// <inheritdoc />
    public string? CorrelationId { get; }
}