using DotaData.Import;
using DotaData.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotaData;

internal class Runner(IHost host, ILogger<Runner> logger, Database db, MatchImporter matchImporter, HeroImporter heroImporter, PlayerImporter playerImporter, PlayerTotalImporter playerTotalImporter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initialising database");
        db.Init();

        await Task.WhenAll([
            heroImporter.Import(stoppingToken),
            playerImporter.Import(stoppingToken),
            playerTotalImporter.Import(stoppingToken),
            matchImporter.Import(stoppingToken)
         ]);

        await host.StopAsync(stoppingToken);
    }
}