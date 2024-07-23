using System.ComponentModel.DataAnnotations;

namespace DotaData.Configuration;

internal class DbSettings
{
    [Required]
    public string? ConnectionString { get; set; }
}