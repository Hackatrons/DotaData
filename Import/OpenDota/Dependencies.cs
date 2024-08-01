using DotaData.Import.Stratz;
using DotaData.OpenDota;
using Microsoft.Extensions.DependencyInjection;

namespace DotaData.Import.OpenDota;

internal static class Dependencies
{
    public static IServiceCollection AddOpenDotaImporter(this IServiceCollection services)
    {
        // add a http client factory with some retry handling
        services
            .AddHttpClient<OpenDotaClient>()
            .AddStandardResilienceHandler();

        services.AddSingleton<OpenDotaImporter>();
        services.AddSingleton<StratzImporter>();
        services.AddSingleton<HeroImporter>();
        services.AddSingleton<PlayerImporter>();
        services.AddSingleton<PlayerTotalImporter>();
        services.AddSingleton<MatchImporter>();

        return services;
    }
}