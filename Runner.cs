using Dapper;
using DotaData.Db;
using DotaData.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;

namespace DotaData;

internal class Runner(IHost host, ILogger<Runner> logger, HttpClient client, Database db) : BackgroundService
{
    const int PlayerIdHack = 18593752;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        db.Init();

        var results = (await new ApiQuery()
            .Player(PlayerIdHack)
            .Matches()
            .Significant(false)
            .Execute<Match>(client, cancellationToken: stoppingToken))
            .ToList();

        logger.LogInformation("Retrieved {count} matches from the API", results.Count);

        await Import(results!, stoppingToken);
        logger.LogInformation("Updated database with new matches");

        await host.StopAsync(stoppingToken);
    }

    async Task Import(IList<Match> matches, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        // delete existing data
        // must do this  before we start a writer
        await connection.ExecuteAsync("truncate table match", transaction: transaction);

        await using var writer = await connection.BeginBinaryImportAsync("copy match (matchid, playerslot, radiantwin, gamemode, heroid, starttime, duration, lobbytype, version, kills, deaths, assists, averagerank, leaverstatus, partysize, herovariant) from stdin (format binary)", cancellationToken);

        // import new data
        foreach (var match in matches)
        {
            await writer.StartRowAsync(cancellationToken);
            await writer.WriteAsync(match.MatchId, cancellationToken);
            await writer.WriteAsync(match.PlayerSlot, cancellationToken);
            await writer.WriteAsync(match.RadiantWin, NpgsqlDbType.Bit, cancellationToken);
            await writer.WriteAsync(match.GameMode, cancellationToken);
            await writer.WriteAsync(match.HeroId, cancellationToken);
            await writer.WriteAsync(match.StartTime, cancellationToken);
            await writer.WriteAsync(match.Duration, cancellationToken);
            await writer.WriteAsync(match.LobbyType, cancellationToken);
            await writer.WriteAsync(match.Version, cancellationToken);
            await writer.WriteAsync(match.Kills, cancellationToken);
            await writer.WriteAsync(match.Deaths, cancellationToken);
            await writer.WriteAsync(match.Assists, cancellationToken);
            await writer.WriteAsync(match.AverageRank, cancellationToken);
            await writer.WriteAsync(match.LeaverStatus, cancellationToken);
            await writer.WriteAsync(match.PartySize, cancellationToken);
            await writer.WriteAsync(match.HeroVariant, cancellationToken);
        }

        await writer.CompleteAsync(cancellationToken);
        await writer.CloseAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}