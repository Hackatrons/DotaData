using Dapper;
using DotaData.Cleansing.Stratz;
using DotaData.Logging;
using DotaData.Persistence;
using DotaData.Stratz;
using DotaData.Stratz.Json;
using Microsoft.Extensions.Logging;
using DotaData.Http;

namespace DotaData.Import.Stratz;

/// <summary>
/// Imports match information to the database.
/// </summary>
internal class PlayerMatchImporter(ILogger<PlayerMatchImporter> logger, StratzClient client, Database db)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var imported = await Task.WhenAll(AccountId.All.Select(id => Import(id, cancellationToken)));
        logger.LogInformation("Imported {rows} new player matches.", imported.Sum());
    }

    async Task<int> Import(int accountId, CancellationToken cancellationToken)
    {
        var apiResults = await client
            .Query()
            .Player(accountId)
            .Matches()
            .GetJsonResults<StratzMatchPlayer>(client, cancellationToken);

        if (apiResults.IsError)
        {
            logger.LogApiError(apiResults.GetError());
            return 0;
        }

        var dbResults = apiResults
            .GetValue()
            .Where(PlayerMatchFilter.IsValid)
            //.Select(x => x.ToDb(accountId))
            .ToList();

        return 0;
    }
}