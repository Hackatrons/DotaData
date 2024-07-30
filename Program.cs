using DotaData;
using DotaData.Configuration;
using DotaData.Import.OpenDota;
using DotaData.Import.Stratz;
using DotaData.OpenDota;
using DotaData.Persistence;
using DotaData.Stratz;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// appsettings.json related
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>();
builder.Services
    .Configure<DbSettings>(builder.Configuration.GetSection("Database"))
    .AddOptionsWithValidateOnStart<DbSettings>()
    .ValidateDataAnnotations();

// add a http client factory with some retry handling
builder.Services
    .AddHttpClient<OpenDotaClient>()
    .AddStandardResilienceHandler();

builder.Services
    .AddHttpClient<StratzClient>()
    .AddStandardResilienceHandler();

// register our classes
builder.Services.AddSingleton<Runner>();
builder.Services.AddSingleton<OpenDotaImporter>();
builder.Services.AddSingleton<StratzImporter>();
builder.Services.AddSingleton<HeroImporter>();
builder.Services.AddSingleton<PlayerImporter>();
builder.Services.AddSingleton<PlayerTotalImporter>();
builder.Services.AddSingleton<PlayerMatchImporter>();
builder.Services.AddSingleton<MatchImporter>();
builder.Services.AddSingleton<DbUpgradeLogger>();
builder.Services.AddSingleton<Database>();

// our worker service
builder.Services.AddHostedService<Runner>();

var host = builder.Build();
await host.RunAsync();
