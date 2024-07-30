using System.ComponentModel.DataAnnotations;

namespace DotaData.Configuration;

internal class StratzSettings
{
    [Required]
    public string? ApiToken { get; set; }
}