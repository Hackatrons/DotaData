namespace DotaData.Persistence.Domain.OpenDota;

internal class OpenDotaPlayerTotal
{
    public int AccountId { get; set; }
    public string? Field { get; set; }
    public double Sum { get; set; }
    public int Count { get; set; }
}