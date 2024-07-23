﻿namespace DotaData.Json;

/// <summary>
/// Represents a json object from the Open Dota API.
/// </summary>
internal class OpenDotaTotal
{
    public string? Field { get; set; }
    public double? Sum { get; set; }
    public int? N { get; set; }
}
