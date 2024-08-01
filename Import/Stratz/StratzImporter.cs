namespace DotaData.Import.Stratz;

internal class StratzImporter(MatchImporter playerMatchImporter)
{
    public async Task Import(CancellationToken stoppingToken)
    {
        await playerMatchImporter.Import(stoppingToken);
    }
}