using DotaData.Persistence;
using DotaData.Stratz;
using Microsoft.Extensions.Logging;

namespace DotaData.Import.Stratz;

/// <summary>
/// Imports match information to the database.
/// </summary>
internal class PlayerMatchImporter(ILogger<PlayerMatchImporter> logger, StratzClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var imported = await Task.WhenAll(AccountId.All.Select(id => Import(id, cancellationToken)));
        logger.LogInformation("Stratz - Imported {rows} new player matches.", imported.Sum());
    }

    async Task<int> Import(int accountId, CancellationToken cancellationToken)
    {
        return 0;
    }
}