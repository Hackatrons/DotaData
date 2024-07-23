using Dapper;
using DotaData.Cleansing;
using DotaData.Logging;
using DotaData.Mapping;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using DotaData.Persistence.Domain;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

/// <summary>
/// Imports match information to the database.
/// </summary>
internal class MatchImporter(ILogger<MatchImporter> logger, HttpClient client, Database db)
{
    // get 5 matches at a time
    const int ChunkSize = 5;
    const int Limit = 100;

    public async Task Import(CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();

        // grab a list of matches to import
        // prefer the most recent
        var ids = (await connection.QueryAsync<dynamic>(
           """
           select distinct MatchId, StartTime
           from dbo.PlayerMatch
           order by StartTime desc
           """)).ToList();

        if (!ids.Any())
            return;

        // TODO: restrict work within the 2,000 daily rate limit
        // for now just grab 100
        var partition = ids.Select(x => (long)x.MatchId).Take(Limit).ToList();
        var chunks = partition.Chunk(ChunkSize);
        var imported = 0;

        foreach (var chunk in chunks)
        {
            var apiResults = await Task.WhenAll(chunk.Select(async id => (await new ApiQuery()
                .Match(id)
                .ExecuteSet<OpenDotaMatch>(client, cancellationToken))));

            // TODO: set a flag in db to exclude this match next time if it's a 404
            var errors = apiResults
                .Where(x => x.IsError)
                .ToList();

            foreach (var error in errors)
            {
                logger.LogApiError(error.GetError());
            }

            var results = apiResults
                .Where(x => x.IsSuccess)
                .Select(x => x.GetValue())
                .SelectMany(x => x)
                .Where(MatchFilter.IsValid)
                .Select(x => x.ToDb())
                .ToList();

            if (!results.Any())
                continue;

            var saved = await Import(results, cancellationToken);
            imported += saved;
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
}