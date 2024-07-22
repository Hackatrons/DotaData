using Dapper;
using DotaData.Cleansing;
using DotaData.Collections;
using DotaData.Db;
using DotaData.Db.Domain;
using DotaData.Json;
using DotaData.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

internal class MatchImporter(ILogger<MatchImporter> logger, HttpClient client, Database db)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        var queries = await Task.WhenAll(AccountId.All.Select(async id => new
        {
            AccountId = id,
            // TODO: maybe run in parallel
            Results = await new ApiQuery()
                .Player(id)
                .Matches()
                .Significant(false)
                .ExecuteSet<OpenDotaMatch>(client, stoppingToken)
        }));

        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();

        var data = queries.Select(x => new
        {
            x.AccountId,
            x.Results,
            DbResults = x.Results
                .Where(MatchFilter.IsValid)
                .Select(MatchMapper.ToDb)
                .ToList()
        }).ToList();

        // union the set of matches together
        var allMatches = data
            .SelectMany(x => x.DbResults).ToHashSet(new LambdaEqualityComparer<Match>((x, y) => x?.MatchId == y?.MatchId, x => x.MatchId.GetHashCode()))
            .ToList();

        var importedMatches = await ImportMatches(allMatches, connection, transaction, stoppingToken);
        logger.LogInformation("Imported {rows} new matches.", importedMatches);

        var importedPlayerMatchLinks = 0;
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var set in data)
        {
            importedPlayerMatchLinks += await CreatePlayerLinks(set.AccountId, set.DbResults, connection, transaction, stoppingToken);
        }

        logger.LogInformation("Imported {rows} new player match links.", importedPlayerMatchLinks);

        await transaction.CommitAsync(stoppingToken);
    }

    static async Task<int> ImportMatches(IList<Match> matches, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync("truncate table Staging.Match", transaction: transaction);

        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = "Staging.Match"
        };

        // must specify the column mappings
        // as by default it uses ordinal positions which may differ between the sql table and the c# type
        bulkCopy.LoadColumnMappings<Match>();

        var dt = matches.ToDataTable();
        await bulkCopy.WriteToServerAsync(dt, cancellationToken);

        // only insert new items that we don't already know about
        return await connection.ExecuteAsync(
             """
             merge Raw.Match as Target
             using Staging.Match as Source
             on Source.MatchId = Target.MatchId
             when not matched by Target then
                insert (
                    MatchId, 
                    PlayerSlot, 
                    RadiantWin, 
                    GameMode, 
                    HeroId, 
                    StartTime, 
                    Duration, 
                    LobbyType, 
                    Version, 
                    Kills, 
                    Deaths, 
                    Assists, 
                    AverageRank, 
                    LeaverStatus, 
                    PartySize, 
                    HeroVariant)
                values (
                    Source.MatchId, 
                    Source.PlayerSlot, 
                    Source.RadiantWin, 
                    Source.GameMode, 
                    Source.HeroId, 
                    Source.StartTime, 
                    Source.Duration, 
                    Source.LobbyType, 
                    Source.Version, 
                    Source.Kills, 
                    Source.Deaths, 
                    Source.Assists, 
                    Source.AverageRank, 
                    Source.LeaverStatus, 
                    Source.PartySize, 
                    Source.HeroVariant);
             """,
            param: null,
            transaction: transaction);
    }

    static async Task<int> CreatePlayerLinks(int accountId, IList<Match> matches, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync("truncate table Staging.PlayerMatch", transaction: transaction);

        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = "Staging.PlayerMatch"
        };

        // must specify the column mappings
        // as by default it uses ordinal positions which may differ between the sql table and the c# type
        bulkCopy.LoadColumnMappings<PlayerMatch>();

        var dt = matches.Select(x => new PlayerMatch { AccountId = accountId, MatchId = x.MatchId}).ToDataTable();
        await bulkCopy.WriteToServerAsync(dt, cancellationToken);

        // only insert new items that we don't already know about
        return await connection.ExecuteAsync(
            """
            merge Raw.PlayerMatch as Target
            using Staging.PlayerMatch as Source
            on Source.AccountId = Target.AccountId and Source.MatchId = Target.MatchId
            when not matched by Target then
               insert (
                   AccountId,
                   MatchId)
               values (
                   Source.AccountId, 
                   Source.MatchId);
            """,
            param: null,
            transaction: transaction);
    }
}