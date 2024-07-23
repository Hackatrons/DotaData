using DotaData.OpenDota.Json;
using DotaData.Persistence.Domain;

namespace DotaData.Mapping;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class HeroMapper
{
    public static Hero ToDb(this OpenDotaHero hero) => new()
    {
        AttackType = hero.AttackType,
        Id = hero.Id,
        Legs = hero.Legs,
        PrimaryAttr = hero.PrimaryAttr,
        LocalizedName = hero.LocalizedName,
        Name = hero.Name
    };
}