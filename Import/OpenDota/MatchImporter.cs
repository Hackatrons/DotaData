using Dapper;
using DotaData.Cleansing.OpenDota;
using DotaData.Http;
using DotaData.Logging;
using DotaData.Mapping.OpenDota;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using DotaData.Persistence.Domain.OpenDota;
using Microsoft.Extensions.Logging;

namespace DotaData.Import.OpenDota;

/// <summary>
/// Imports match information to the database.
/// </summary>
internal class MatchImporter(ILogger<MatchImporter> logger, OpenDotaClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var imported = await Task.WhenAll(AccountId.All.Select(id => Import(id, cancellationToken)));
        logger.LogInformation("Imported {count} new matches", imported);
    }

    async Task<int> Import(int accountId, CancellationToken cancellationToken)
    {
        var playerMatches = await client
            .Query()
            .Player(accountId)
            .Matches()
            .Significant(false)
            .GetJsonResults<PlayerMatch>(client, cancellationToken);

        if (playerMatches.IsError)
        {
            logger.LogApiError(playerMatches.GetError());
            return 0;
        }

        var idsToQuery = await GetMatchesToQuery(
            accountId, playerMatches.GetValue()
            .Where(x => x.MatchId is not null)
            .Select(x => x.MatchId.GetValueOrDefault()));

        await using var connection = db.CreateConnection();

        var imported = 0;

        // I've tried running this in parallel but the server is very quick to send 429's back
        // safer to just run 1 at a time
        foreach (var id in idsToQuery)
        {
            var apiResults = await client.Query()
                .Match(id)
                .GetJsonResults<Match>(client, cancellationToken);

            if (apiResults.IsError)
            {
                var error = apiResults.GetError();
                var httpError = error as HttpRequestException;

                logger.LogApiError(error);

                await connection.ExecuteAsync(
                    """
                        insert into OpenDota.MatchImport (MatchId, Success, ErrorCode, ErrorMessage, Timestamp)
                        values (@MatchId, @Success, @ErrorCode, @ErrorMessage, @Timestamp)
                    """, param: new
                    {
                        MatchId = id,
                        Success = false,
                        ErrorCode = (int?)httpError?.StatusCode,
                        ErrorMessage = error.Message,
                        Timestamp = DateTime.UtcNow
                    });

                if (httpError?.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // we're sending too fast, wait a minute
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }

                continue;
            }

            var results = apiResults.GetValue()
                .Where(MatchFilter.IsValid)
                .Select(x => x.ToDb())
                .ToList();

#pragma warning disable CA1860
            if (!results.Any())
#pragma warning restore CA1860
                continue;

            var saved = await Import(results, cancellationToken);
            imported += saved;

            var details = apiResults.GetValue()
                .Where(MatchFilter.IsValid)
                .SelectMany(x => x.Players ?? [])
                .Where(MatchPlayerDetailFilter.IsValid)
                .Select(x => x.ToDb(id))
                .ToList();

#pragma warning disable CA1860
            if (!details.Any())
#pragma warning restore CA1860
                continue;

            await Import(details, cancellationToken);
        }

        return imported;
    }

    async Task<int> Import(IEnumerable<OpenDotaMatch> matches, CancellationToken cancellationToken)
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

    async Task<int> Import(IEnumerable<OpenDotaMatchPlayer> details, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(details, "OpenDotaStaging.MatchPlayer", transaction, cancellationToken);

        var affected = await connection.ExecuteAsync(
            """
            merge OpenDota.MatchPlayer as Target
            using OpenDotaStaging.MatchPlayer as Source
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

    async Task<IEnumerable<long>> GetMatchesToQuery(int playerId, IEnumerable<long> allPlayerMatches)
    {
        await using var connection = db.CreateConnection();

        // grab the set of matches already imported or to retry
        // if we got a 429 response, then wait 1 day to try again
        var toExclude = await connection.QueryAsync<long>(
            """
            select distinct MatchId
            from OpenDota.MatchPlayer MP
            where AccountId = @AccountId
            and MatchId not in
            (
                select MatchId from OpenDota.MatchImport
                where (ErrorCode != 429 or datediff(day, getutcdate(), [Timestamp]) = 0)
            )
            """, param: new { AccountId = playerId });

        return allPlayerMatches.Except(toExclude);
    }
}