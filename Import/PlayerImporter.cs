using Dapper;
using DotaData.Cleansing;
using DotaData.Db;
using DotaData.Db.Domain;
using DotaData.Json;
using DotaData.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

internal class PlayerImporter(ILogger<PlayerImporter> logger, HttpClient client, Database db)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        var data = (await Task.WhenAll(AccountId.All.Select(async id =>
            await new ApiQuery()
                .Player(id)
                .Significant(false)
                .ExecuteSingle<OpenDotaPlayer>(client, stoppingToken)
        )))
        .Where(PlayerFilter.IsValid)
        .ToList();

        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();

        var players = data.Select(x => x.ToDb()).ToList();
        var imported = await ImportPlayers(players, connection, transaction, stoppingToken);

        logger.LogInformation("Imported {rows} players.", imported);

        await transaction.CommitAsync(stoppingToken);
    }

    static async Task<int> ImportPlayers(IList<Player> players, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync("truncate table Staging.Player", transaction: transaction);

        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = "Staging.Player"
        };

        bulkCopy.LoadColumnMappings<Player>();

        var dt = players.ToDataTable();
        await bulkCopy.WriteToServerAsync(dt, cancellationToken);

        // only insert new items that we don't already know about
        // TODO: update existing when matched
        return await connection.ExecuteAsync(
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
    }
}