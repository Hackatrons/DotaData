using Dapper;
using DotaData.Cleansing;
using DotaData.Db;
using DotaData.Db.Domain;
using DotaData.Db.Mapping;
using DotaData.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DotaData.Import;

internal class HeroImporter(ILogger<HeroImporter> logger, HttpClient client, Database db)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        await using var connection = db.CreateConnection();

        var populated = await connection.ExecuteAsync("select count(*) from Raw.Hero", stoppingToken);

        if (populated > 0)
            return;

        var results = (await new ApiQuery()
            .Heroes()
            .Significant(false)
            .Execute<OpenDotaHero>(client, stoppingToken))
            .Where(HeroFilter.IsValid)
            .Select(HeroMapper.ToDb)
            .ToList();

        await using var transaction = connection.BeginTransaction();

        await ImportHeroes(results, connection, transaction, stoppingToken);
        logger.LogInformation("Imported heroes.");

        await transaction.CommitAsync(stoppingToken);
    }

    static async Task ImportHeroes(IList<Hero> matches, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync("truncate table Raw.Hero", transaction: transaction);

        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = "Raw.Hero"
        };

        bulkCopy.LoadColumnMappings<Hero>();

        var dt = matches.ToDataTable();
        await bulkCopy.WriteToServerAsync(dt, cancellationToken);
    }
}