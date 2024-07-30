using Microsoft.Extensions.DependencyInjection;

namespace DotaData.Persistence;

internal static class Dependencies
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<DbUpgradeLogger>();
        services.AddSingleton<Database>();

        return services;
    }
}