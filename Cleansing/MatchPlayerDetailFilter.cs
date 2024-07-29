using DotaData.OpenDota.Json;

namespace DotaData.Cleansing;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class MatchPlayerDetailFilter
{
    public static bool IsValid(OpenDotaMatchPlayerDetail detail) => detail.AccountId is not null;
}