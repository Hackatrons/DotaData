using DotaData.OpenDota.Json;

namespace DotaData.Cleansing.OpenDota;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class HeroFilter
{
    public static bool IsValid(Hero hero) =>
        hero.Id is not null &&
        hero.AttackType is not null &&
        hero.Name is not null &&
        hero.LocalizedName is not null &&
        hero.PrimaryAttr is not null;
}