using DotaData.Stratz.Json;

namespace DotaData.Cleansing.Stratz;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class MatchPlayerFilter
{
    public static bool IsValid(MatchPlayer player) => player.SteamAccountId is not null && player.MatchId is not null;
}