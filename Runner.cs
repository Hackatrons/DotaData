﻿using DotaData.Db;
using DotaData.Import;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotaData;

internal class Runner(IHost host, ILogger<Runner> logger, Database db, MatchImporter matchImporter, HeroImporter heroImporter, PlayerImporter playerImporter, PlayerTotalImporter playerTotalImporter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initialising database");
        db.Init();

        await heroImporter.Import(stoppingToken);
        await playerImporter.Import(stoppingToken);
        await playerTotalImporter.Import(stoppingToken);
        await matchImporter.Import(stoppingToken);

        await host.StopAsync(stoppingToken);
    }
}