using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotaData.Configuration;

internal static class Dependencies
{
    public static HostApplicationBuilder AddAppSettings(this HostApplicationBuilder builder)
    {
        // appsettings.json related
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<Program>();
        builder.Services
            .Configure<DbSettings>(builder.Configuration.GetSection("Database"))
            .AddOptionsWithValidateOnStart<DbSettings>()
            .ValidateDataAnnotations();

        return builder;
    }
}