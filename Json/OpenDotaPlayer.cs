namespace DotaData.Json;

internal class OpenDotaPlayer
{
    public long? SoloCompetitiveRank { get; set; }
    public long? CompetitiveRank { get; set; }
    public long? RankTier { get; set; }
    public long? LeaderboardRank { get; set; }
    public OpenDotaProfile? Profile { get; set; }
}