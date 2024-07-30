namespace DotaData.Import.Stratz;

internal class StratzImporter()
{
    public async Task Import(CancellationToken stoppingToken)
    {
        // import order is important as we have key constraints to meet
        await Task.Yield();
    }
}