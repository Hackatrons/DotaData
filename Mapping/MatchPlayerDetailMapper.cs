using DotaData.OpenDota.Json;
using DotaData.Persistence.Domain;

namespace DotaData.Mapping;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class MatchPlayerDetailMapper
{
    public static MatchPlayerDetail ToDb(this OpenDotaMatchPlayerDetail detail, long matchId) => new()
    {
        AccountId = detail.AccountId ?? throw new ArgumentNullException(nameof(detail), "AccountId cannot be null"),
        MatchId = matchId,
        Abandons = detail.Abandons,
        AbilityUpgradesArr = detail.AbilityUpgradesArr is not null ? string.Join(",", detail.AbilityUpgradesArr) : null,
        AghanimsScepter = detail.AghanimsScepter,
        AghanimsShard = detail.AghanimsShard,
        Assists = detail.Assists,
        Backpack0 = detail.Backpack0,
        Backpack1 = detail.Backpack1,
        Backpack2 = detail.Backpack2,
        Cluster = detail.Cluster,
        Deaths = detail.Deaths,
        Denies = detail.Denies,
        Gold = detail.Gold,
        GoldPerMin = detail.GoldPerMin,
        GoldSpent = detail.GoldSpent,
        HeroDamage = detail.HeroDamage,
        HeroHealing = detail.HeroHealing,
        HeroId = detail.HeroId,
        HeroVariant = detail.HeroVariant,
        IsRadiant = detail.IsRadiant,
        Item0 = detail.Item0,
        Item1 = detail.Item1,
        Item2 = detail.Item2,
        Item3 = detail.Item3,
        Item4 = detail.Item4,
        Item5 = detail.Item5,
        ItemNeutral = detail.ItemNeutral,
        Kda = detail.Kda,
        Kills = detail.Kills,
        KillsPerMin = detail.KillsPerMin,
        LastHits = detail.LastHits,
        LeaverStatus = detail.LeaverStatus,
        Level = detail.Level,
        Moonshard = detail.Moonshard,
        NetWorth = detail.NetWorth,
        PersonaName = detail.PersonaName,
        PlayerSlot = detail.PlayerSlot,
        RadiantWin = detail.RadiantWin,
        RankTier = detail.RankTier,
        TeamNumber = detail.TeamNumber,
        TeamSlot = detail.TeamSlot,
        TotalGold = detail.TotalGold,
        TotalXp = detail.TotalXp,
        TowerDamage = detail.TowerDamage,
        XpPerMin = detail.XpPerMin,
    };
}