using DotaData.Db.Domain;
using DotaData.Json;

namespace DotaData.Mapping;

internal static class HeroMapper
{
    public static Hero ToDb(this OpenDotaHero hero) => new()
    {
        AttackType = hero.AttackType,
        Id = hero.Id,
        Legs = hero.Legs,
        PrimaryAttr = hero.PrimaryAttr,
        LocalizedName = hero.LocalizedName,
        Name = hero.Name,
    };
}