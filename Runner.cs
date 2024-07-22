using DotaData.Db;
using DotaData.Import;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotaData;

internal class Runner(IHost host, ILogger<Runner> logger, Database db, MatchImporter matchImporter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initialising database");
        db.Init();

        logger.LogInformation("Importing matches");
        await matchImporter.Import(stoppingToken);

        await host.StopAsync(stoppingToken);
    }
}