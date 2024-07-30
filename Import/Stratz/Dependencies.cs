using System.Net.Http.Headers;
using DotaData.Configuration;
using DotaData.Stratz;
using Microsoft.Extensions.DependencyInjection;

namespace DotaData.Import.Stratz;

internal static class Dependencies
{
    public static IServiceCollection AddStratzImporter(this IServiceCollection services, StratzSettings settings)
    {
        // add a http client factory with some retry handling
        services
            .AddHttpClient<StratzClient>()
            .ConfigureHttpClient(x =>
            {
                x.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiToken);
            })
            .AddStandardResilienceHandler();

        services.AddSingleton<StratzImporter>();
        services.AddSingleton<PlayerMatchImporter>();

        return services;
    }
}