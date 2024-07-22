using Dapper;
using DotaData.Db;
using DotaData.Db.Domain;
using DotaData.Db.Mapping;
using DotaData.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DotaData.Import;

internal class MatchImporter(ILogger<MatchImporter> logger, HttpClient client, Database db)
{
    const int PlayerIdHack = 18593752;

    public async Task Import(CancellationToken stoppingToken)
    {
        var results = (await new ApiQuery()
            .Player(PlayerIdHack)
            .Matches()
            .Significant(false)
            .Execute<OpenDotaMatch>(client, cancellationToken: stoppingToken))
            .Select(MatchMapper.ToDb)
            .ToList();

        logger.LogInformation("Retrieved {count} matches from the API", results.Count);

        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();

        await DeleteExistingData(connection, transaction);
        await ImportMatches(results, connection, transaction, stoppingToken);
        await CreatePlayerLinks(PlayerIdHack, results, connection, transaction, stoppingToken);

        await transaction.CommitAsync(stoppingToken);
        logger.LogInformation("Updated database with new matches");
    }

    static async Task DeleteExistingData(IDbConnection connection, IDbTransaction transaction)
    {
        await connection.ExecuteAsync("truncate table Match", transaction: transaction);
        await connection.ExecuteAsync("truncate table PlayerMatch", transaction: transaction);
    }

    static async Task ImportMatches(IList<Match> matches, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = "Match"
        };

        // must specify the column mappings
        // as by default it uses ordinal positions which may differ between the sql table and the c# type
        bulkCopy.LoadColumnMappings<Match>();

        var dt = matches.ToDataTable();
        await bulkCopy.WriteToServerAsync(dt, cancellationToken);
    }

    static async Task CreatePlayerLinks(int playerId, IList<Match> matches, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = "PlayerMatch"
        };

        bulkCopy.LoadColumnMappings<PlayerMatch>();

        var dt = matches.Select(x => new PlayerMatch { PlayerId = playerId, MatchId = x.MatchId ?? throw new InvalidDataException("MatchId cannot be null") }).ToDataTable();
        await bulkCopy.WriteToServerAsync(dt, cancellationToken);
    }
}