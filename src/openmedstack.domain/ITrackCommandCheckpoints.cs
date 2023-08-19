namespace OpenMedStack.Domain;

/// <summary>
/// Defines the public interface for tracking command commit checkpoints.
/// The checkpoint tracker is expected to track checkpoints for a single tenant only.
/// </summary>
public interface ITrackCommandCheckpoints : ITrackCheckpoints { }
