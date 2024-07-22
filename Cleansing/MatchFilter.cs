using DotaData.Json;

namespace DotaData.Cleansing;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class MatchFilter
{
    public static bool IsValid(OpenDotaMatch match)
    {
        return match.MatchId is not null;
    }
}