using Dapper;
using DotaData.Cleansing.Stratz;
using DotaData.Logging;
using DotaData.Persistence;
using DotaData.Stratz;
using DotaData.Stratz.Json;
using Microsoft.Extensions.Logging;
using DotaData.Http;
using DotaData.ErrorHandling;
using DotaData.Mapping.Stratz;
using DotaData.Persistence.Domain.Stratz;

namespace DotaData.Import.Stratz;

/// <summary>
/// Imports match information to the database.
/// </summary>
internal class PlayerMatchImporter(ILogger<PlayerMatchImporter> logger, StratzClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var imported = await Task.WhenAll(AccountId.All.Select(id => Import(id, cancellationToken)));
        logger.LogInformation("Imported {rows} new player matches.", imported.Sum());
    }

    async Task<int> Import(int accountId, CancellationToken cancellationToken)
    {
        int position;
        var imported = 0;
        ValueOrError<IEnumerable<Match>> apiResults;

        await using (var connection = db.CreateConnection())
        {
            position = await connection.QuerySingleAsync<int>(
                "select count(*) from Stratz.MatchPlayer where SteamAccountId = @Id",
                param: new { Id = accountId });
        }

        do
        {
            apiResults = await client
                .Query()
                .Player(accountId)
                .Matches()
                .Skip(position)
                .TakeMax()
                .GetJsonResults<Match>(client, cancellationToken);

            if (apiResults.IsError)
            {
                logger.LogApiError(apiResults.GetError());
                return imported;
            }

            var apiResultsList = apiResults.GetValue().ToList();
            var dbMatchResults = apiResults
                .GetValue()
                .Where(MatchFilter.IsValid)
                .Select(x => x.ToDb())
                .ToList();
            var dbMatchPlayerResults = apiResults
                .GetValue()
                .Where(MatchFilter.IsValid)
                .SelectMany(x => x.Players ?? [])
                .Where(MatchPlayerFilter.IsValid)
                .Select(x => x.ToDb())
                .ToList();

            position += apiResultsList.Count;
            imported += await Save(dbMatchResults, cancellationToken);
            await Save(dbMatchPlayerResults, cancellationToken);
        }
        while (apiResults.IsSuccess && apiResults.GetValue().Any());

        return imported;
    }

    async Task<int> Save(IEnumerable<StratzMatch> items, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(items, "StratzStaging.Match", transaction, cancellationToken);

        // only insert new items that we don't already know about
        var affected = await connection.ExecuteAsync(
            """
            merge Stratz.Match as Target
            using StratzStaging.Match as Source
            on Source.Id = Target.Id
            when not matched by Target then
               insert (
                    [Id]
                    ,[DidRadiantWin]
                    ,[DurationSeconds]
                    ,[StartDateTime]
                    ,[ClusterId]
                    ,[FirstBloodTime]
                    ,[LobbyType]
                    ,[NumHumanPlayers]
                    ,[GameMode]
                    ,[IsStats]
                    ,[GameVersionId]
                    ,[RegionId]
                    ,[SequenceNum]
                    ,[Rank]
                    ,[Bracket]
                    ,[EndDateTime]
                    ,[DidRequestDownload]
                    ,[AvgImp]
                    ,[ParsedDateTime]
                    ,[StatsDateTime]
                    ,[AnalysisOutcome]
                    ,[PredictedOutcomeWeight]
                    ,[BottomLaneOutcome]
                    ,[MidLaneOutcome]
                    ,[TopLaneOutcome]
                    ,[RadiantNetworthLead]
                    ,[RadiantExperienceLead]
                    ,[RadiantKills]
                    ,[DireKills]
                    ,[WinRates]
                    ,[PredictedWinRates])
                values (
                     Source.[Id]
                    ,Source.[DidRadiantWin]
                    ,Source.[DurationSeconds]
                    ,Source.[StartDateTime]
                    ,Source.[ClusterId]
                    ,Source.[FirstBloodTime]
                    ,Source.[LobbyType]
                    ,Source.[NumHumanPlayers]
                    ,Source.[GameMode]
                    ,Source.[IsStats]
                    ,Source.[GameVersionId]
                    ,Source.[RegionId]
                    ,Source.[SequenceNum]
                    ,Source.[Rank]
                    ,Source.[Bracket]
                    ,Source.[EndDateTime]
                    ,Source.[DidRequestDownload]
                    ,Source.[AvgImp]
                    ,Source.[ParsedDateTime]
                    ,Source.[StatsDateTime]
                    ,Source.[AnalysisOutcome]
                    ,Source.[PredictedOutcomeWeight]
                    ,Source.[BottomLaneOutcome]
                    ,Source.[MidLaneOutcome]
                    ,Source.[TopLaneOutcome]
                    ,Source.[RadiantNetworthLead]
                    ,Source.[RadiantExperienceLead]
                    ,Source.[RadiantKills]
                    ,Source.[DireKills]
                    ,Source.[WinRates]
                    ,Source.[PredictedWinRates]);
            """,
            param: null,
            transaction: transaction);

        await transaction.CommitAsync(cancellationToken);

        return affected;
    }

    async Task<int> Save(IEnumerable<StratzMatchPlayer> items, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(items, "StratzStaging.MatchPlayer", transaction, cancellationToken);

        // only insert new items that we don't already know about
        var affected = await connection.ExecuteAsync(
            """
            merge Stratz.MatchPlayer as Target
            using StratzStaging.MatchPlayer as Source
            on Source.MatchId = Target.MatchId and Source.SteamAccountId = Target.SteamAccountId
            when not matched by Target then
               insert (
                    [MatchId]
                    ,[PlayerSlot]
                    ,[HeroId]
                    ,[SteamAccountId]
                    ,[IsRadiant]
                    ,[NumKills]
                    ,[NumDeaths]
                    ,[NumAssists]
                    ,[LeaverStatus]
                    ,[NumLastHits]
                    ,[NumDenies]
                    ,[GoldPerMinute]
                    ,[ExperiencePerMinute]
                    ,[Level]
                    ,[Gold]
                    ,[GoldSpent]
                    ,[HeroDamage]
                    ,[TowerDamage]
                    ,[PartyId]
                    ,[Item0Id]
                    ,[Item1Id]
                    ,[Item2Id]
                    ,[Item3Id]
                    ,[Item4Id]
                    ,[Item5Id]
                    ,[HeroHealing]
                    ,[IsVictory]
                    ,[Networth]
                    ,[Neutral0Id]
                    ,[Variant]
                    ,[IsRandom]
                    ,[Lane]
                    ,[IntentionalFeeding]
                    ,[Role]
                    ,[Imp]
                    ,[Award]
                    ,[Behavior]
                    ,[RoamLane]
                    ,[DotaPlusHeroXp]
                    ,[InvisibleSeconds])
                values (
                     Source.[MatchId]
                    ,Source.[PlayerSlot]
                    ,Source.[HeroId]
                    ,Source.[SteamAccountId]
                    ,Source.[IsRadiant]
                    ,Source.[NumKills]
                    ,Source.[NumDeaths]
                    ,Source.[NumAssists]
                    ,Source.[LeaverStatus]
                    ,Source.[NumLastHits]
                    ,Source.[NumDenies]
                    ,Source.[GoldPerMinute]
                    ,Source.[ExperiencePerMinute]
                    ,Source.[Level]
                    ,Source.[Gold]
                    ,Source.[GoldSpent]
                    ,Source.[HeroDamage]
                    ,Source.[TowerDamage]
                    ,Source.[PartyId]
                    ,Source.[Item0Id]
                    ,Source.[Item1Id]
                    ,Source.[Item2Id]
                    ,Source.[Item3Id]
                    ,Source.[Item4Id]
                    ,Source.[Item5Id]
                    ,Source.[HeroHealing]
                    ,Source.[IsVictory]
                    ,Source.[Networth]
                    ,Source.[Neutral0Id]
                    ,Source.[Variant]
                    ,Source.[IsRandom]
                    ,Source.[Lane]
                    ,Source.[IntentionalFeeding]
                    ,Source.[Role]
                    ,Source.[Imp]
                    ,Source.[Award]
                    ,Source.[Behavior]
                    ,Source.[RoamLane]
                    ,Source.[DotaPlusHeroXp]
                    ,Source.[InvisibleSeconds]);
            """,
            param: null,
            transaction: transaction);

        await transaction.CommitAsync(cancellationToken);

        return affected;
    }
}