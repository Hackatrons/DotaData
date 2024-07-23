using DotaData.OpenDota.Json;

namespace DotaData.Cleansing;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class PlayerFilter
{
    public static bool IsValid(OpenDotaPlayer player) => player.Profile?.AccountId is not null;
}