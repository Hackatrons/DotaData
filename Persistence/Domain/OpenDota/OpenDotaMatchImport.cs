namespace DotaData.Persistence.Domain.OpenDota;

internal class OpenDotaMatchImport
{
    public long MatchId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ErrorCode { get; set; }
}