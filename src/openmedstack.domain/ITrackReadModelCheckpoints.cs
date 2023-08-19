namespace OpenMedStack.Domain;

/// <summary>
/// Defines the public interface for tracking commit checkpoints.
/// The checkpoint tracker is expected to track checkpoints for a single tenant only.
/// </summary>
public interface ITrackReadModelCheckpoints : ITrackCheckpoints { }
