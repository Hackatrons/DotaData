using DotaData.Import.OpenDota;
using DotaData.Import.Stratz;
using DotaData.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotaData;

internal class Runner(
    IHost host,
    ILogger<Runner> logger,
    Database db,
    OpenDotaImporter openDotaImporter,
    StratzImporter stratzImporter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initialising database");
        db.Init();

        await Task.WhenAll(
            //openDotaImporter.Import(stoppingToken),
            stratzImporter.Import(stoppingToken));

        await host.StopAsync(stoppingToken);
    }
}