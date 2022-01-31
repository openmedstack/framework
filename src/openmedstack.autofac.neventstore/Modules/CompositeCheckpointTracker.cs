namespace OpenMedStack.Autofac.NEventstore.Modules;

using System.Linq;
using System.Threading.Tasks;
using OpenMedStack.Domain;

internal class CompositeCheckpointTracker : ITrackCheckpoints
{
    private readonly ITrackCheckpoints[] _trackers;

    public CompositeCheckpointTracker(params ITrackCheckpoints[] trackers)
    {
        _trackers = trackers;
    }

    /// <inheritdoc />
    public async Task<long> GetLatest()
    {
        var tasks = _trackers.Select(x => x.GetLatest());
        return (await Task.WhenAll(tasks).ConfigureAwait(false)).Min();
    }

    /// <inheritdoc />
    public async Task SetLatest(long value)
    {
        foreach (var tracker in _trackers)
        {
            if (await tracker.GetLatest().ConfigureAwait(false) < value)
            {
                await tracker.SetLatest(value).ConfigureAwait(false);
            }
        }
    }
}