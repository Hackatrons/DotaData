using DotaData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<Runner>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Runner>();

var host = builder.Build();
await host.RunAsync();
