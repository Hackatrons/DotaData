using DotaData.OpenDota.Json;
using DotaData.Persistence.Domain.OpenDota;

namespace DotaData.Mapping.OpenDota;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class MatchMapper
{
    public static Match ToDb(this OpenDotaMatch match) => new()
    {
        BarracksStatusDire = match.BarracksStatusDire,
        BarracksStatusRadiant = match.BarracksStatusRadiant,
        Cluster = match.Cluster,
        DireScore = match.DireScore,
        Duration = match.Duration,
        Engine = match.Engine,
        FirstBloodTime = match.FirstBloodTime,
        Flags = match.Flags,
        GameMode = match.GameMode,
        HumanPlayers = match.HumanPlayers,
        LeagueId = match.LeagueId,
        LobbyType = match.LobbyType,
        MatchId = match.MatchId ?? throw new ArgumentNullException(nameof(match), "MatchId cannot be null"),
        MatchSeqNum = match.MatchSeqNum,
        Patch = match.Patch,
        PreGameDuration = match.PreGameDuration,
        RadiantScore = match.RadiantScore,
        RadiantWin = match.RadiantWin,
        Region = match.Region,
        StartTime = match.StartTime,
        TowerStatusDire = match.TowerStatusDire,
        TowerStatusRadiant = match.TowerStatusRadiant,
    };
}