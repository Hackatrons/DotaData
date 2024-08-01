namespace DotaData.Persistence.Domain.OpenDota;

internal class OpenDotaHero
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string? LocalizedName { get; set; }
    public string? PrimaryAttr { get; set; }
    public string? AttackType { get; set; }
    public int? Legs { get; set; }
}
