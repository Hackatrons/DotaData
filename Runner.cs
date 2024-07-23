using DotaData.Import;
using DotaData.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotaData;

internal class Runner(
    IHost host, 
    ILogger<Runner> logger,
    Database db,
    PlayerMatchImporter playerMatchImporter,
    HeroImporter heroImporter, 
    PlayerImporter playerImporter, 
    PlayerTotalImporter playerTotalImporter,
    MatchImporter matchImporter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initialising database");
        db.Init();

        // import order is important as we have key constraints to meet
        await heroImporter.Import(stoppingToken);
        await playerImporter.Import(stoppingToken);

        await Task.WhenAll([
            playerTotalImporter.Import(stoppingToken),
            playerMatchImporter.Import(stoppingToken),
         ]);

        await matchImporter.Import(stoppingToken);
        await host.StopAsync(stoppingToken);
    }
}