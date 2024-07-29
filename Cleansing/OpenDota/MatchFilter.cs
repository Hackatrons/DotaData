using DotaData.OpenDota.Json;

namespace DotaData.Cleansing.OpenDota;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class MatchFilter
{
    public static bool IsValid(OpenDotaMatch match) => match.MatchId is not null;
}