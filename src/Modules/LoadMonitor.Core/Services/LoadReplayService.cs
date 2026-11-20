using LoadMonitor.Core.Index;
using OrchardCore.Modules;
using YesSql;

namespace LoadMonitor.Core.Services;

// Replays data from AI training data for reference, like it's a real time measurement, interface needed for real time measurement
public sealed class LoadReplayService(ISession session, IClock clock)
{
    public const int StepSeconds = 2;

    public async Task<IReadOnlyList<string>> GetProfilesAsync()
    {
        var firstSamples = await session.QueryIndex<LoadSampleIndex>(x => x.Sequence == 0).ListAsync();
        return firstSamples.Select(x => x.Profile).Distinct().OrderBy(x => x).ToList();
    }    
    public async Task<IReadOnlyList<double>> GetRecentAsync(string profile, string assetId, int count)
    {
        var samples = (await session.QueryIndex<LoadSampleIndex>(x => x.Profile == profile)
            .OrderBy(x => x.Sequence)
            .ListAsync()).ToList();

        if (samples.Count == 0)
        {
            return [];
        }

        var current = CurrentPosition(samples.Count, assetId);
        count = Math.Min(count, samples.Count);

        return Enumerable.Range(0, count)
            .Select(i => samples[(current - count + 1 + i + samples.Count) % samples.Count].PowerW)
            .ToList();
    }

    public async Task<double?> GetCurrentAsync(string profile, string assetId)
    {
        var recent = await GetRecentAsync(profile, assetId, 1);
        return recent.Count > 0 ? recent[0] : null;
    }

    private int CurrentPosition(int sampleCount, string assetId)
    {
        var step = (long)(clock.UtcNow - DateTime.UnixEpoch).TotalSeconds / StepSeconds;
        var offset = assetId.Aggregate(0, (hash, c) => unchecked(hash * 31 + c)) & int.MaxValue;

        return (int)((step + offset) % sampleCount);
    }
}
