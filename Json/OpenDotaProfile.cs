using System.Text.Json.Serialization;

namespace DotaData.Json;

/// <summary>
/// Represents a json object from the Open Dota API.
/// </summary>
internal class OpenDotaProfile
{
    public long? AccountId { get; set; }
    [JsonPropertyName("personaname")]
    public string? PersonaName { get; set; }
    public string? Name { get; set; }
    public bool? Plus { get; set; }
    public long? Cheese { get; set; }
    [JsonPropertyName("steamid")]
    public string? SteamId { get; set; }
    public string? Avatar { get; set; }
    [JsonPropertyName("avatarmedium")]
    public string? AvatarMedium { get; set; }
    [JsonPropertyName("avatarfull")]
    public string? AvatarFull { get; set; }
    [JsonPropertyName("profileurl")]
    public string? ProfileUrl { get; set; }
    public int? LastLogin { get; set; }
    [JsonPropertyName("loccountrycode")]
    public string? LocCountryCode { get; set; }
    public string? Status { get; set; }
    public bool? FhUnavailable { get; set; }
    public bool? IsContributor { get; set; }
    public bool? IsSubscriber { get; set; }
}