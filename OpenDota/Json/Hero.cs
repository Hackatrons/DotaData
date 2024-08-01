namespace DotaData.OpenDota.Json;

/// <summary>
/// Represents a json object from the Open Dota API.
/// </summary>
internal class Hero
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string? LocalizedName { get; set; }
    public string? PrimaryAttr { get; set; }
    public string? AttackType { get; set; }
    public int? Legs { get; set; }
}
