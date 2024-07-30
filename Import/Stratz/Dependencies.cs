using DotaData.Stratz;
using Microsoft.Extensions.DependencyInjection;

namespace DotaData.Import.Stratz;

internal static class Dependencies
{
    public static IServiceCollection AddStratzImporter(this IServiceCollection services)
    {
        // add a http client factory with some retry handling
        services
            .AddHttpClient<StratzClient>()
            .AddStandardResilienceHandler();

        services.AddSingleton<StratzImporter>();
        services.AddSingleton<PlayerMatchImporter>();

        return services;
    }
}