﻿using DotaData.Json;

namespace DotaData.Cleansing;

/// <summary>
/// Determines whether a result is good data or not (and should be discarded).
/// </summary>
internal static class PlayerFilter
{
    public static bool IsValid(OpenDotaPlayer player)
    {
        return player.Profile?.AccountId is not null;
    }
}