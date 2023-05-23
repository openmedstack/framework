namespace OpenMedStack.Domain;

using System.Threading.Tasks;

/// <summary>
/// Defines the public interface for tracking commit checkpoints.
/// The checkpoint tracker is expected to track checkpoints for a single tenant only.
/// </summary>
public interface ITrackCheckpoints
{
    /// <summary>
    /// Gets or sets the latest checkpoint for the tenant.
    /// </summary>
    /// <returns></returns>
    Task<long> GetLatest();

    Task SetLatest(long value);
}
