namespace DotaData.Import.OpenDota;

internal class OpenDotaImporter(
    PlayerMatchImporter playerMatchImporter,
    HeroImporter heroImporter, 
    PlayerImporter playerImporter, 
    PlayerTotalImporter playerTotalImporter,
    MatchImporter matchImporter)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        // import order is important as we have key constraints to meet
        await heroImporter.Import(stoppingToken);
        await playerImporter.Import(stoppingToken);

        await Task.WhenAll([
            playerTotalImporter.Import(stoppingToken),
            playerMatchImporter.Import(stoppingToken),
         ]);

        await matchImporter.Import(stoppingToken);
    }
}