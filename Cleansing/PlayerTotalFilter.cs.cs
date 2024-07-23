using DotaData.OpenDota.Json;

namespace DotaData.Cleansing;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class PlayerTotalFilter
{
    public static bool IsValid(OpenDotaTotal total) => !string.IsNullOrEmpty(total.Field) && total.Sum is not null && total.N is not null;
}