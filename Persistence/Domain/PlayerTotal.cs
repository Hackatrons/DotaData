namespace DotaData.Persistence.Domain;

internal class PlayerTotal
{
    public int AccountId { get; set; }
    public string? Field { get; set; }
    public double Sum {get;set; }
    public int Count {get;set; }
}