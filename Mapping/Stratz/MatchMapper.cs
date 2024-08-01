using DotaData.Persistence.Domain.Stratz;
using DotaData.Stratz.Json;

namespace DotaData.Mapping.Stratz;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class MatchMapper
{
    public static StratzMatch ToDb(this Match match) => new()
    {
        AnalysisOutcome = match.AnalysisOutcome,
        AvgImp = match.AvgImp,
        BottomLaneOutcome = match.BottomLaneOutcome,
        Bracket = match.Bracket,
        ClusterId = match.ClusterId,
        DidRadiantWin = match.DidRadiantWin,
        DidRequestDownload = match.DidRequestDownload,
        DireKills = match.DireKills is not null ? string.Join(",", match.DireKills) : null,
        DurationSeconds = match.DurationSeconds,
        EndDateTime = match.EndDateTime,
        FirstBloodTime = match.FirstBloodTime,
        GameMode = match.GameMode,
        GameVersionId = match.GameVersionId,
        Id = match.Id ?? throw new ArgumentNullException(nameof(match), "Id cannot be null"),
        IsStats = match.IsStats,
        LobbyType = match.LobbyType,
        MidLaneOutcome = match.MidLaneOutcome,
        NumHumanPlayers = match.NumHumanPlayers,
        ParsedDateTime = match.ParsedDateTime,
        PredictedOutcomeWeight = match.PredictedOutcomeWeight,
        PredictedWinRates = match.PredictedWinRates is not null ? string.Join(",", match.PredictedWinRates) : null,
        RadiantExperienceLead = match.RadiantExperienceLead is not null ? string.Join(",", match.RadiantExperienceLead) : null,
        RadiantKills = match.RadiantKills is not null ? string.Join(",", match.RadiantKills) : null,
        RadiantNetworthLead = match.RadiantNetworthLead is not null ? string.Join(",", match.RadiantNetworthLead) : null,
        Rank = match.Rank,
        RegionId = match.RegionId,
        SequenceNum = match.SequenceNum,
        StartDateTime = match.StartDateTime,
        StatsDateTime = match.StatsDateTime,
        TopLaneOutcome = match.TopLaneOutcome,
        WinRates = match.WinRates is not null ? string.Join(",", match.WinRates) : null
    };
}