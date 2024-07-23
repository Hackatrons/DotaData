using Dapper;
using DotaData.Cleansing;
using DotaData.Logging;
using DotaData.Mapping;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

/// <summary>
/// Imports hero information to the database.
/// </summary>
internal class HeroImporter(ILogger<HeroImporter> logger, HttpClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();

        var populated = await connection.ExecuteScalarAsync<int>("select count(*) from dbo.Hero", cancellationToken);

        if (populated > 0)
            return;

        var apiResults = await new ApiQuery()
            .Heroes()
            .Significant(false)
            .ExecuteSet<OpenDotaHero>(client, cancellationToken);

        if (!apiResults.IsSuccess)
        {
            logger.LogApiError(apiResults.GetError());
            return;
        }

        var dbResults = apiResults
            .GetValue()
            .Where(HeroFilter.IsValid)
            .Select(HeroMapper.ToDb)
            .ToList();

        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(dbResults, "dbo.Hero", transaction, cancellationToken, true);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Imported {count} heroes.", dbResults.Count);
    }
}