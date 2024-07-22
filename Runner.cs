using DotaData.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotaData
{
    internal class Runner(IHost host, ILogger<Runner> logger, HttpClient client) : BackgroundService
    {
        const int PlayerIdHack = 18593752;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var results = await new ApiQuery()
                .Player(PlayerIdHack)
                .Matches()
                .Significant(false)
                .Execute<Match>(client, cancellationToken: stoppingToken);

            logger.LogInformation("Done {count}", results.Count());
            await host.StopAsync(stoppingToken);
        }
    }
}
