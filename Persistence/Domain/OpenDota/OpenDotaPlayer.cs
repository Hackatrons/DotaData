namespace DotaData.Persistence.Domain.OpenDota;

internal class OpenDotaPlayer
{
    public long AccountId { get; set; }
    public string? PersonaName { get; set; }
    public string? Name { get; set; }
    public bool? Plus { get; set; }
    public long? Cheese { get; set; }
    public string? SteamId { get; set; }
    public string? Avatar { get; set; }
    public string? AvatarMedium { get; set; }
    public string? AvatarFull { get; set; }
    public string? ProfileUrl { get; set; }
    public int? LastLogin { get; set; }
    public string? LocCountryCode { get; set; }
    public string? Status { get; set; }
    public bool? FhUnavailable { get; set; }
    public bool? IsContributor { get; set; }
    public bool? IsSubscriber { get; set; }
    public long? SoloCompetitiveRank { get; set; }
    public long? CompetitiveRank { get; set; }
    public long? RankTier { get; set; }
    public long? LeaderboardRank { get; set; }
}