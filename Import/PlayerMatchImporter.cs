using Dapper;
using DotaData.Cleansing;
using DotaData.Mapping;
using DotaData.OpenDota;
using DotaData.OpenDota.Json;
using DotaData.Persistence;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

/// <summary>
/// Imports match information to the database.
/// </summary>
internal class PlayerMatchImporter(ILogger<PlayerMatchImporter> logger, HttpClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var imported = await Task.WhenAll(AccountId.All.Select(id => Import(id, cancellationToken)));
        logger.LogInformation("Imported {rows} new player matches.", imported.Sum());
    }

    async Task<int> Import(int accountId, CancellationToken cancellationToken)
    {
        var results = (await new ApiQuery()
            .Player(accountId)
            .Matches()
            .Significant(false)
            .ExecuteSet<OpenDotaPlayerMatch>(client, cancellationToken))
            .Where(PlayerMatchFilter.IsValid)
            .Select(x => x.ToDb(accountId))
            .ToList();

        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();
        await connection.BulkLoad(results, "Staging.PlayerMatch", transaction, cancellationToken);

        // only insert new items that we don't already know about
        var affected = await connection.ExecuteAsync(
             """
             merge Raw.PlayerMatch as Target
             using Staging.PlayerMatch as Source
             on Source.MatchId = Target.MatchId
             when not matched by Target then
                insert (
                    MatchId, 
                    AccountId,
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
                    Source.AccountId,
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

        await transaction.CommitAsync(cancellationToken);

        return affected;
    }
}