using Dapper;
using DotaData.Cleansing;
using DotaData.Mapping;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

/// <summary>
/// Imports player information to the database.
/// </summary>
internal class PlayerImporter(ILogger<PlayerImporter> logger, HttpClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var imported = await Task.WhenAll(AccountId.All.Select(id => Import(id, cancellationToken)));

        logger.LogInformation("Imported {count} players.", imported.Sum());
    }

    async Task<int> Import(int accountId, CancellationToken cancellationToken)
    {
        var apiResult = await new ApiQuery()
                    .Player(accountId)
                    .Significant(false)
                    .ExecuteSingle<OpenDotaPlayer>(client, cancellationToken);

        if (!PlayerFilter.IsValid(apiResult))
        {
            logger.LogError("Invalid {type} data for {account}", nameof(OpenDotaPlayer), accountId);
            return 0;
        }

        var dbResult = apiResult.ToDb();

        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad([dbResult], "Staging.Player", transaction, cancellationToken);

        // TODO: update existing when matched
        var affected = await connection.ExecuteAsync(
            """
            merge Raw.Player as Target
            using Staging.Player as Source
            on Source.AccountId = Target.AccountId
            when not matched by Target then
                insert (
                    [AccountId]
                    ,[PersonaName]
                    ,[Name]
                    ,[Cheese]
                    ,[SteamId]
                    ,[Avatar]
                    ,[AvatarMedium]
                    ,[AvatarFull]
                    ,[ProfileUrl]
                    ,[LastLogin]
                    ,[LocCountryCode]
                    ,[Status]
                    ,[FhUnavailable]
                    ,[IsContributor]
                    ,[IsSubscriber]
                    ,[RankTier]
                    ,[SoloCompetitiveRank]
                    ,[CompetitiveRank]
                    ,[LeaderboardRank])   
                values (
                     Source.[AccountId]
                    ,Source.[PersonaName]
                    ,Source.[Name]
                    ,Source.[Cheese]
                    ,Source.[SteamId]
                    ,Source.[Avatar]
                    ,Source.[AvatarMedium]
                    ,Source.[AvatarFull]
                    ,Source.[ProfileUrl]
                    ,Source.[LastLogin]
                    ,Source.[LocCountryCode]
                    ,Source.[Status]
                    ,Source.[FhUnavailable]
                    ,Source.[IsContributor]
                    ,Source.[IsSubscriber]
                    ,Source.[RankTier]
                    ,Source.[SoloCompetitiveRank]
                    ,Source.[CompetitiveRank]
                    ,Source.[LeaderboardRank]);
            """,
            param: null,
            transaction: transaction);

        await transaction.CommitAsync(cancellationToken);

        return affected;
    }
}