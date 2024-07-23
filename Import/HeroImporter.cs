﻿using Dapper;
using DotaData.Cleansing;
using DotaData.Db;
using DotaData.Db.Domain;
using DotaData.Json;
using DotaData.Mapping;
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
            .ExecuteSet<OpenDotaHero>(client, stoppingToken))
            .Where(HeroFilter.IsValid)
            .Select(HeroMapper.ToDb);

        await using var transaction = connection.BeginTransaction();

        await ImportHeroes(results, connection, transaction, stoppingToken);
        logger.LogInformation("Imported heroes.");

        await transaction.CommitAsync(stoppingToken);
    }

    static async Task ImportHeroes(IEnumerable<Hero> matches, SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.BulkLoad(matches, "Raw.Hero", transaction, cancellationToken);
    }
}