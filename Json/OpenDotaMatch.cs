namespace DotaData.Json;

/// <summary>
/// Represents a json object from the Open Dota API.
/// </summary>
internal class OpenDotaMatch
{
    public long? MatchId { get; set; }
    public int? PlayerSlot { get; set; }
    public bool? RadiantWin { get; set; }
    public int? GameMode { get; set; }
    public int? HeroId { get; set; }
    public int? StartTime { get; set; }
    public int? Duration { get; set; }
    public int? LobbyType { get; set; }
    public int? Version { get; set; }
    public int? Kills { get; set; }
    public int? Deaths { get; set; }
    public int? Assists { get; set; }
    public int? AverageRank { get; set; }
    public int? LeaverStatus { get; set; }
    public int? PartySize { get; set; }
    public int? HeroVariant { get; set; }
}
