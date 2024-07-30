namespace DotaData.Import.Stratz;

internal class StratzImporter(PlayerMatchImporter playerMatchImporter)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        await playerMatchImporter.Import(stoppingToken);
    }
}