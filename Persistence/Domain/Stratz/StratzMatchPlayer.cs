namespace DotaData.Persistence.Domain.Stratz;

internal class StratzMatchPlayer
{
    public long MatchId { get; set; }
    public int? PlayerSlot { get; set; }
    public int? HeroId { get; set; }
    public int SteamAccountId { get; set; }
    public bool? IsRadiant { get; set; }
    public int? NumKills { get; set; }
    public int? NumDeaths { get; set; }
    public int? NumAssists { get; set; }
    public int? LeaverStatus { get; set; }
    public int? NumLastHits { get; set; }
    public int? NumDenies { get; set; }
    public int? GoldPerMinute { get; set; }
    public int? ExperiencePerMinute { get; set; }
    public int? Level { get; set; }
    public int? Gold { get; set; }
    public int? GoldSpent { get; set; }
    public int? HeroDamage { get; set; }
    public int? TowerDamage { get; set; }
    public int? PartyId { get; set; }
    public int? Item0Id { get; set; }
    public int? Item1Id { get; set; }
    public int? Item2Id { get; set; }
    public int? Item3Id { get; set; }
    public int? Item4Id { get; set; }
    public int? Item5Id { get; set; }
    public int? HeroHealing { get; set; }
    public bool? IsVictory { get; set; }
    public int? Networth { get; set; }
    public int? Neutral0Id { get; set; }
    public int? Variant { get; set; }
    public bool? IsRandom { get; set; }
    public int? Lane { get; set; }
    public bool? IntentionalFeeding { get; set; }
    public int? Role { get; set; }
    public double? Imp { get; set; }
    public int? Award { get; set; }
    public int? Behavior { get; set; }
    public int? RoamLane { get; set; }
    public int? DotaPlusHeroXp { get; set; }
    public int? InvisibleSeconds { get; set; }
}