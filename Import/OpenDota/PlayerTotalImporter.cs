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
/// Imports player performance metrics to the database.
/// </summary>
internal class PlayerTotalImporter(ILogger<PlayerTotalImporter> logger, OpenDotaClient client, Database db)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        var imported = await Task.WhenAll(AccountId.All.Select(id => Import(id, stoppingToken)));

        logger.LogInformation("Imported {rows} new player totals.", imported.Sum());
    }

    async Task<int> Import(int accountId, CancellationToken cancellationToken)
    {
        var apiResults = await client
            .Query()
            .Player(accountId)
            .Totals()
            .Significant(false)
            .GetJsonResults<OpenDotaTotal>(client, cancellationToken);

        if (apiResults.IsError)
        {
            logger.LogApiError(apiResults.GetError());
            return 0;
        }

        var dbResults = apiResults
            .GetValue()
            .Where(PlayerTotalFilter.IsValid)
            .Select(result => result.ToDb(accountId))
            .ToList();

        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();

        await connection.BulkLoad(dbResults, "OpenDotaStaging.PlayerTotal", transaction, cancellationToken);

        // only insert new items that we don't already know about
        var affected = await connection.ExecuteAsync(
            """
            merge OpenDota.PlayerTotal as Target
            using OpenDotaStaging.PlayerTotal as Source
            on Source.AccountId = Target.AccountId and Source.Field = Target.Field
            when not matched by Target then
               insert (
                   AccountId,
                   Field,
                   [Count],
                   [Sum])
               values (
                   Source.AccountId, 
                   Source.Field,
                   Source.[Count],
                   Source.[Sum])
            when matched and (Target.Count != Source.Count or Target.Sum != Source.Sum) then
                update set [Count] = Source.Count, [Sum] = Source.Sum;
            """,
            param: null,
            transaction: transaction);

        await transaction.CommitAsync(cancellationToken);

        return affected;
    }
}