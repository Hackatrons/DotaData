using DotaData.Json;

namespace DotaData.Cleansing;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class TotalFilter
{
    public static bool IsValid(OpenDotaTotal total)
    {
        return !string.IsNullOrEmpty(total.Field) && total.Sum is not null && total.N is not null;
    }
}