using DotaData;
using DotaData.Configuration;
using DotaData.Db;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Database"));

builder.Services.AddSingleton<Runner>();
builder.Services.AddSingleton<DbUpgradeLogger>();
builder.Services.AddSingleton<Database>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Runner>();

var host = builder.Build();
await host.RunAsync();
