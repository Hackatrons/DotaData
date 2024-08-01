﻿namespace DotaData.Persistence.Domain.OpenDota;

internal class MatchImport
{
    public long MatchId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ErrorCode { get; set; }
}