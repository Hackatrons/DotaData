﻿using DotaData;
using DotaData.Configuration;
using DotaData.Import.OpenDota;
using DotaData.Import.Stratz;
using DotaData.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.AddAppSettings();
builder.Services.AddDatabase();
builder.Services.AddOpenDotaImporter();
builder.Services.AddStratzImporter(builder.Services.BuildServiceProvider().GetRequiredService<IOptions<StratzSettings>>().Value);
builder.Services.AddHostedService<Runner>();

var host = builder.Build();
await host.RunAsync();
