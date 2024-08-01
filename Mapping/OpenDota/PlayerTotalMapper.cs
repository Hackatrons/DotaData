using DotaData.Persistence.Domain.OpenDota;
using DotaData.OpenDota.Json;

namespace DotaData.Mapping.OpenDota;

/// <summary>
/// Maps API objects to the database equivalent.
/// </summary>
internal static class PlayerTotalMapper
{
    public static OpenDotaPlayerTotal ToDb(this Total total, int accountId) => new()
    {
        AccountId = accountId,
        Field = total.Field ?? throw new ArgumentNullException(nameof(total), "Field cannot be null"),
        Count = total.N ?? throw new ArgumentNullException(nameof(total), "N cannot be null"),
        Sum = total.Sum ?? throw new ArgumentNullException(nameof(total), "Sum cannot be null")
    };
}