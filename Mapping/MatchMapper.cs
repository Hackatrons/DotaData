using DotaData.OpenDota.Json;
using DotaData.Persistence.Domain;

namespace DotaData.Mapping;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class MatchMapper
{
    public static Match ToDb(this OpenDotaMatch match) => new()
    {
        Assists = match.Assists,
        AverageRank = match.AverageRank,
        Deaths = match.Deaths,
        Duration = match.Duration,
        GameMode = match.GameMode,
        HeroId = match.HeroId,
        HeroVariant = match.HeroVariant,
        Kills = match.Kills,
        LeaverStatus = match.LeaverStatus,
        LobbyType = match.LobbyType,
        MatchId = match.MatchId ?? throw new ArgumentNullException(nameof(match), "MatchId cannot be null"),
        PartySize = match.PartySize,
        PlayerSlot = match.PlayerSlot,
        RadiantWin = match.RadiantWin,
        StartTime = match.StartTime,
        Version = match.Version
    };
}