namespace DotaData.Db.Domain;

internal class PlayerTotal
{
    public int PlayerId { get; set; }
    public string? Field { get; set; }
    public double Sum {get;set; }
    public int Count {get;set; }
}