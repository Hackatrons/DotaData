using Dapper;
using DotaData.Cleansing;
using DotaData.Logging;
using DotaData.Mapping;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using DotaData.Persistence.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

/// <summary>
/// Imports match information to the database.
/// </summary>
internal class MatchImporter(ILogger<MatchImporter> logger, OpenDotaClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();

        var ids = (await GetMatchesToQuery(connection)).ToList();
        var imported = 0;

        while (ids.Any())
        {
            // I've tried running this in parallel but the server is very quick to send 429's back
            // seems safer to just run 1 at a time
            foreach (var id in ids)
            {
                var apiResults = await new ApiQuery()
                    .Match(id)
                    .ExecuteSet<OpenDotaMatch>(client, cancellationToken);

                if (apiResults.IsError)
                {
                    var error = apiResults.GetError();
                    var httpError = error as HttpRequestException;

                    logger.LogApiError(error);

                    if (httpError?.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        // we're sending too fast, wait a minute
                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                        continue;
                    }

                    await connection.ExecuteAsync(
                        """
                        insert into dbo.MatchImport (MatchId, Success, ErrorCode, ErrorMessage)
                        values (@MatchId, @Success, @ErrorCode, @ErrorMessage)
                        """, param: new
                        {
                            MatchId = id,
                            Success = false,
                            ErrorCode = (int?)httpError?.StatusCode,
                            ErrorMessage = error.Message,
                        });

                    continue;
                }

                var results = apiResults.GetValue()
                    .Where(MatchFilter.IsValid)
                    .Select(x => x.ToDb())
                    .ToList();

                if (!results.Any())
                    continue;

                var saved = await Import(results, cancellationToken);
                imported += saved;
            }

            ids = (await GetMatchesToQuery(connection)).ToList();
        }

        logger.LogInformation("Imported {count} new matches", imported);
    }

    async Task<int> Import(IEnumerable<Match> matches, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(matches, "Staging.Match", transaction, cancellationToken);

        var affected = await connection.ExecuteAsync(
            """
            merge dbo.Match as Target
            using Staging.Match as Source
            on Source.MatchId = Target.MatchId
            when not matched by Target then
               insert (
                    [MatchId]
                    ,[RadiantWin]
                    ,[Duration]
                    ,[PreGameDuration]
                    ,[StartTime]
                    ,[MatchSeqNum]
                    ,[TowerStatusRadiant]
                    ,[TowerStatusDire]
                    ,[BarracksStatusRadiant]
                    ,[BarracksStatusDire]
                    ,[Cluster]
                    ,[FirstBloodTime]
                    ,[LobbyType]
                    ,[HumanPlayers]
                    ,[LeagueId]
                    ,[GameMode]
                    ,[Flags]
                    ,[Engine]
                    ,[RadiantScore]
                    ,[DireScore]
                    ,[Patch]
                    ,[Region])
                values (
                    Source.[MatchId]
                    ,Source.[RadiantWin]
                    ,Source.[Duration]
                    ,Source.[PreGameDuration]
                    ,Source.[StartTime]
                    ,Source.[MatchSeqNum]
                    ,Source.[TowerStatusRadiant]
                    ,Source.[TowerStatusDire]
                    ,Source.[BarracksStatusRadiant]
                    ,Source.[BarracksStatusDire]
                    ,Source.[Cluster]
                    ,Source.[FirstBloodTime]
                    ,Source.[LobbyType]
                    ,Source.[HumanPlayers]
                    ,Source.[LeagueId]
                    ,Source.[GameMode]
                    ,Source.[Flags]
                    ,Source.[Engine]
                    ,Source.[RadiantScore]
                    ,Source.[DireScore]
                    ,Source.[Patch]
                    ,Source.[Region]);
            """,
            param: null,
            transaction: transaction);

        await transaction.CommitAsync(cancellationToken);

        return affected;
    }

    static async Task<IEnumerable<long>> GetMatchesToQuery(SqlConnection connection)
    {
        // grab a list of matches to import
        // prefer the most recent
        // exclude those we've already tried
        var toQuery = await connection.QueryAsync<dynamic>(
            """
                select distinct top 100 PM.MatchId, PM.StartTime
                from dbo.PlayerMatch PM
                left join dbo.Match M on M.MatchId = PM.MatchId
                left join dbo.MatchImport MI on MI.MatchId = PM.MatchId
                where M.MatchId is null and MI.MatchId is null
                order by StartTime desc
            """);

        return toQuery.Select(x => (long)x.MatchId);
    }
}