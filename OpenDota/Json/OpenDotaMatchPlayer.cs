﻿using System.Text.Json.Serialization;

namespace DotaData.OpenDota.Json;

internal class OpenDotaMatchPlayerDetail
{
    public int? AccountId { get; set; }
    public int? PlayerSlot { get; set; }
    public int? TeamNumber { get; set; }
    public int? TeamSlot { get; set; }
    public int? HeroId { get; set; }
    public int? HeroVariant { get; set; }
    [JsonPropertyName("item_0")]
    public int? Item0 { get; set; }
    [JsonPropertyName("item_1")]
    public int? Item1 { get; set; }
    [JsonPropertyName("item_2")]
    public int? Item2 { get; set; }
    [JsonPropertyName("item_3")]
    public int? Item3 { get; set; }
    [JsonPropertyName("item_4")]
    public int? Item4 { get; set; }
    [JsonPropertyName("item_5")]
    public int? Item5 { get; set; }
    [JsonPropertyName("backpack_0")]
    public int? Backpack0 { get; set; }
    [JsonPropertyName("backpack_1")]
    public int? Backpack1 { get; set; }
    [JsonPropertyName("backpack_2")]
    public int? Backpack2 { get; set; }
    public int? ItemNeutral { get; set; }
    public int? Kills { get; set; }
    public int? Deaths { get; set; }
    public int? Assists { get; set; }
    public int? LeaverStatus { get; set; }
    public int? LastHits { get; set; }
    public int? Denies { get; set; }
    public int? GoldPerMin { get; set; }
    public int? XpPerMin { get; set; }
    public int? Level { get; set; }
    public int? NetWorth { get; set; }
    public int? AghanimsScepter { get; set; }
    public int? AghanimsShard { get; set; }
    public int? Moonshard { get; set; }
    public int? HeroDamage { get; set; }
    public int? TowerDamage { get; set; }
    public int? HeroHealing { get; set; }
    public int? Gold { get; set; }
    public int? GoldSpent { get; set; }
    public int[]? AbilityUpgradesArr { get; set; }
    [JsonPropertyName("personaname")]
    public string? PersonaName { get; set; }
    public bool? RadiantWin { get; set; }
    public int? Cluster { get; set; }
    [JsonPropertyName("isRadiant")]
    public bool? IsRadiant { get; set; }
    public int? TotalGold { get; set; }
    public int? TotalXp { get; set; }
    public double? KillsPerMin { get; set; }
    public double? Kda { get; set; }
    public int? Abandons { get; set; }
    public int? RankTier { get; set; }
}