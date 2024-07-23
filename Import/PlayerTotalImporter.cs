using Dapper;
using DotaData.Cleansing;
using DotaData.Db;
using DotaData.Db.Domain;
using DotaData.Json;
using DotaData.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

internal class PlayerTotalImporter(ILogger<PlayerTotalImporter> logger, HttpClient client, Database db)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        var queries = await Task.WhenAll(AccountId.All.Select(async id => new
        {
            AccountId = id,
            // TODO: maybe run in parallel
            Results = await new ApiQuery()
                .Player(id)
                .Totals()
                .Significant(false)
                .ExecuteSet<OpenDotaTotal>(client, stoppingToken)
        }));

        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();

        var data = queries.Select(x => new
        {
            x.AccountId,
            x.Results,
            DbResults = x.Results
                .Where(PlayerTotalFilter.IsValid)
                .Select(result => result.ToDb(x.AccountId))
                .ToList()
        }).ToList();

        var importedPlayerMatchLinks = 0;
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var set in data)
        {
            importedPlayerMatchLinks += await ImportPlayerTotals(set.DbResults, connection, transaction, stoppingToken);
        }

        logger.LogInformation("Imported {rows} new player totals.", importedPlayerMatchLinks);

        await transaction.CommitAsync(stoppingToken);
    }

    static async Task<int> ImportPlayerTotals(IEnumerable<PlayerTotal> totals, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.BulkLoad(totals, "Staging.PlayerTotal", transaction, cancellationToken);

        // only insert new items that we don't already know about
        return await connection.ExecuteAsync(
            """
            merge Raw.PlayerTotal as Target
            using Staging.PlayerTotal as Source
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
    }
}