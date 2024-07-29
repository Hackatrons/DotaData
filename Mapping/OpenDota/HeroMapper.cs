using DotaData.OpenDota.Json;
using DotaData.Persistence.Domain.OpenDota;

namespace DotaData.Mapping.OpenDota;

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