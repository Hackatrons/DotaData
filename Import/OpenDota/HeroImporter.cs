using Dapper;
using DotaData.Cleansing.OpenDota;
using DotaData.Http;
using DotaData.Logging;
using DotaData.Mapping.OpenDota;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using Microsoft.Extensions.Logging;

namespace DotaData.Import.OpenDota;

/// <summary>
/// Imports hero information to the database.
/// </summary>
internal class HeroImporter(ILogger<HeroImporter> logger, OpenDotaClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();

        var populated = await connection.ExecuteScalarAsync<int>("select count(*) from Reference.Hero", cancellationToken);

        if (populated > 0)
            return;

        var apiResults = await client
            .Query()
            .Heroes()
            .Significant(false)
            .GetJsonResults<OpenDotaHero>(client, cancellationToken);

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
        await connection.BulkLoad(dbResults, "Reference.Hero", transaction, cancellationToken, true);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Imported {count} heroes.", dbResults.Count);
    }
}