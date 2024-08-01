using DotaData.Persistence.Domain.Stratz;
using DotaData.Stratz.Json;

namespace DotaData.Mapping.Stratz;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class MatchPlayerMapper
{
    public static StratzMatchPlayer ToDb(this MatchPlayer match) => new()
    {
        Award = match.Award,
        Behavior = match.Behavior,
        DotaPlusHeroXp = match.DotaPlusHeroXp,
        ExperiencePerMinute = match.ExperiencePerMinute,
        Gold = match.Gold,
        GoldPerMinute = match.GoldPerMinute,
        GoldSpent = match.GoldSpent,
        HeroDamage = match.HeroDamage,
        HeroHealing = match.HeroHealing,
        HeroId = match.HeroId,
        Imp = match.Imp,
        IntentionalFeeding = match.IntentionalFeeding,
        InvisibleSeconds = match.InvisibleSeconds,
        IsRadiant = match.IsRadiant,
        IsRandom = match.IsRandom,
        IsVictory = match.IsVictory,
        Item0Id = match.Item0Id,
        Item1Id = match.Item1Id,
        Item2Id = match.Item2Id,
        Item3Id = match.Item3Id,
        Item4Id = match.Item4Id,
        Item5Id = match.Item5Id,
        Lane = match.Lane,
        LeaverStatus = match.LeaverStatus,
        Level = match.Level,
        MatchId = match.MatchId ?? throw new ArgumentNullException(nameof(match), "MatchId cannot be null"),
        Networth = match.Networth,
        Neutral0Id = match.Neutral0Id,
        NumAssists = match.NumAssists,
        NumDeaths = match.NumDeaths,
        NumDenies = match.NumDenies,
        NumKills = match.NumKills,
        NumLastHits = match.NumLastHits,
        PartyId = match.PartyId,
        PlayerSlot = match.PlayerSlot,
        RoamLane = match.RoamLane,
        Role = match.Role,
        SteamAccountId = match.SteamAccountId ?? throw new ArgumentNullException(nameof(match), "SteamAccountId cannot be null"),
        TowerDamage = match.TowerDamage,
        Variant = match.Variant,
    };
}