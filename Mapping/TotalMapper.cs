using DotaData.Db.Domain;
using DotaData.Json;

namespace DotaData.Mapping;

internal static class TotalMapper
{
    public static PlayerTotal ToDb(this OpenDotaTotal total, int playerId) => new()
    {
        PlayerId = playerId,
        Field = total.Field ?? throw new ArgumentNullException(nameof(total), "Field cannot be null"),
        Count = total.N ?? throw new ArgumentNullException(nameof(total), "N cannot be null"),
        Sum = total.Sum ?? throw new ArgumentNullException(nameof(total), "Sum cannot be null")
    };
}