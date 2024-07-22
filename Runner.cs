using Dapper;
using DotaData.Db;
using DotaData.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            .Execute<OpenDotaMatch>(client, cancellationToken: stoppingToken))
            .ToList();

        logger.LogInformation("Retrieved {count} matches from the API", results.Count);

        await Import(results!, stoppingToken);
        logger.LogInformation("Updated database with new matches");

        await host.StopAsync(stoppingToken);
    }

    async Task Import(IList<OpenDotaMatch> matches, CancellationToken cancellationToken)
    {
        await using var connection = db.CreateConnection();
        await using var transaction = connection.BeginTransaction();

        // delete existing data
        // must do this  before we start a writer
        await connection.ExecuteAsync("truncate table Match", transaction: transaction);

        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = "Match"
        };

        // must specify the column mappings
        // as by default it uses ordinal positions which may differ between the sql table and the c# type
        bulkCopy.LoadColumnMappings<OpenDotaMatch>();

        var dt = matches.ToDataTable();
        await bulkCopy.WriteToServerAsync(dt,  cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}