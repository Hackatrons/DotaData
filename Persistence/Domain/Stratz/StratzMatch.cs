namespace DotaData.Persistence.Domain.Stratz;

internal class StratzMatch
{
    public long Id { get; set; }
    public bool? DidRadiantWin { get; set; }
    public int? DurationSeconds { get; set; }
    public int? StartDateTime { get; set; }
    public int? ClusterId { get; set; }
    public int? FirstBloodTime { get; set; }
    public int? LobbyType { get; set; }
    public int? NumHumanPlayers { get; set; }
    public int? GameMode { get; set; }
    public bool? IsStats { get; set; }
    public int? GameVersionId { get; set; }
    public int? RegionId { get; set; }
    public long? SequenceNum { get; set; }
    public int? Rank { get; set; }
    public int? Bracket { get; set; }
    public int? EndDateTime { get; set; }
    public bool? DidRequestDownload { get; set; }
    public double? AvgImp { get; set; }
    public int? ParsedDateTime { get; set; }
    public int? StatsDateTime { get; set; }
    public int? AnalysisOutcome { get; set; }
    public int? PredictedOutcomeWeight { get; set; }
    public int? BottomLaneOutcome { get; set; }
    public int? MidLaneOutcome { get; set; }
    public int? TopLaneOutcome { get; set; }
    public string? RadiantNetworthLead { get; set; }
    public string? RadiantExperienceLead { get; set; }
    public string? RadiantKills { get; set; }
    public string? DireKills { get; set; }
    public string? WinRates { get; set; }
    public string? PredictedWinRates { get; set; }
}