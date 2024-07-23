using DotaData.Json;

namespace DotaData.Cleansing;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class HeroFilter
{
    public static bool IsValid(OpenDotaHero hero) =>
        hero.Id is not null &&
        hero.AttackType is not null &&
        hero.Name is not null &&
        hero.LocalizedName is not null &&
        hero.PrimaryAttr is not null;
}