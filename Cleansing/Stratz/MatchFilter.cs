using DotaData.Stratz.Json;

namespace DotaData.Cleansing.Stratz;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class MatchFilter
{
    public static bool IsValid(Match match) => match.Id is not null;
}