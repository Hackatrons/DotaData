using Dapper;
using DotaData.Cleansing.OpenDota;
using DotaData.Logging;
using DotaData.Mapping.OpenDota;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using DotaData.Persistence.Domain.OpenDota;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DotaData.Import.OpenDota;

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
                        insert into OpenDota.MatchImport (MatchId, Success, ErrorCode, ErrorMessage)
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

                var details = apiResults.GetValue()
                    .Where(MatchFilter.IsValid)
                    .SelectMany(x => x.Players ?? [])
                    .Where(MatchPlayerDetailFilter.IsValid)
                    .Select(x => x.ToDb(id))
                    .ToList();

                if (!details.Any())
                    continue;

                await Import(details, cancellationToken);
            }

            ids = (await GetMatchesToQuery(connection)).ToList();
        }

        logger.LogInformation("Imported {count} new matches", imported);
    }

    async Task<int> Import(IEnumerable<Match> matches, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(matches, "OpenDotaStaging.Match", transaction, cancellationToken);

        var affected = await connection.ExecuteAsync(
            """
            merge OpenDota.Match as Target
            using OpenDotaStaging.Match as Source
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
                    ,Source.[Region]
                );
            """,
            param: null,
            transaction: transaction);

        await transaction.CommitAsync(cancellationToken);

        return affected;
    }

    async Task<int> Import(IEnumerable<MatchPlayerDetail> details, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(details, "OpenDotaStaging.MatchPlayerDetail", transaction, cancellationToken);

        var affected = await connection.ExecuteAsync(
            """
            merge OpenDota.MatchPlayerDetail as Target
            using OpenDotaStaging.MatchPlayerDetail as Source
            on Source.MatchId = Target.MatchId and Source.AccountId = Target.AccountId
            when not matched by Target then
               insert (
                    [MatchId]
                    ,[AccountId]
                    ,[PlayerSlot]
                    ,[TeamNumber]
                    ,[TeamSlot]
                    ,[HeroId]
                    ,[HeroVariant]
                    ,[Item0]
                    ,[Item1]
                    ,[Item2]
                    ,[Item3]
                    ,[Item4]
                    ,[Item5]
                    ,[Backpack0]
                    ,[Backpack1]
                    ,[Backpack2]
                    ,[ItemNeutral]
                    ,[Kills]
                    ,[Deaths]
                    ,[Assists]
                    ,[LeaverStatus]
                    ,[LastHits]
                    ,[Denies]
                    ,[GoldPerMin]
                    ,[XpPerMin]
                    ,[Level]
                    ,[NetWorth]
                    ,[AghanimsScepter]
                    ,[AghanimsShard]
                    ,[Moonshard]
                    ,[HeroDamage]
                    ,[TowerDamage]
                    ,[HeroHealing]
                    ,[Gold]
                    ,[GoldSpent]
                    ,[AbilityUpgradesArr]
                    ,[PersonaName]
                    ,[RadiantWin]
                    ,[Cluster]
                    ,[IsRadiant]
                    ,[TotalGold]
                    ,[TotalXp]
                    ,[KillsPerMin]
                    ,[Kda]
                    ,[Abandons]
                    ,[RankTier]
               )
               values (
                     Source.[MatchId]
                    ,Source.[AccountId]
                    ,Source.[PlayerSlot]
                    ,Source.[TeamNumber]
                    ,Source.[TeamSlot]
                    ,Source.[HeroId]
                    ,Source.[HeroVariant]
                    ,Source.[Item0]
                    ,Source.[Item1]
                    ,Source.[Item2]
                    ,Source.[Item3]
                    ,Source.[Item4]
                    ,Source.[Item5]
                    ,Source.[Backpack0]
                    ,Source.[Backpack1]
                    ,Source.[Backpack2]
                    ,Source.[ItemNeutral]
                    ,Source.[Kills]
                    ,Source.[Deaths]
                    ,Source.[Assists]
                    ,Source.[LeaverStatus]
                    ,Source.[LastHits]
                    ,Source.[Denies]
                    ,Source.[GoldPerMin]
                    ,Source.[XpPerMin]
                    ,Source.[Level]
                    ,Source.[NetWorth]
                    ,Source.[AghanimsScepter]
                    ,Source.[AghanimsShard]
                    ,Source.[Moonshard]
                    ,Source.[HeroDamage]
                    ,Source.[TowerDamage]
                    ,Source.[HeroHealing]
                    ,Source.[Gold]
                    ,Source.[GoldSpent]
                    ,Source.[AbilityUpgradesArr]
                    ,Source.[PersonaName]
                    ,Source.[RadiantWin]
                    ,Source.[Cluster]
                    ,Source.[IsRadiant]
                    ,Source.[TotalGold]
                    ,Source.[TotalXp]
                    ,Source.[KillsPerMin]
                    ,Source.[Kda]
                    ,Source.[Abandons]
                    ,Source.[RankTier]
               );
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
                from OpenDota.PlayerMatch PM
                left join OpenDota.Match M on M.MatchId = PM.MatchId
                left join OpenDota.MatchImport MI on MI.MatchId = PM.MatchId
                where M.MatchId is null and MI.MatchId is null
                order by StartTime desc
            """);

        return toQuery.Select(x => (long)x.MatchId);
    }
}