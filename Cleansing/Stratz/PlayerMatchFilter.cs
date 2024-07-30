using DotaData.Stratz.Json;

namespace DotaData.Cleansing.Stratz;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class PlayerMatchFilter
{
    public static bool IsValid(StratzMatchPlayer player) => player.SteamAccountId is not null && player.MatchId is not null;
}