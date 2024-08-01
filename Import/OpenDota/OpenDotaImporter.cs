namespace DotaData.Import.OpenDota;

internal class OpenDotaImporter(
    HeroImporter heroImporter,
    PlayerImporter playerImporter,
    PlayerTotalImporter playerTotalImporter,
    MatchImporter matchImporter)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        // import order is important as we have key constraints to meet
        await Task.WhenAll(
            heroImporter.Import(stoppingToken),
            playerImporter.Import(stoppingToken));

        await Task.WhenAll(
            playerTotalImporter.Import(stoppingToken),
            matchImporter.Import(stoppingToken));
    }
}