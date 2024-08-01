﻿using System.Net;
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
                x.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DotaData", "1.0"));
            })
            //.ConfigurePrimaryHttpMessageHandler(() =>
            //{
            //    var cookieContainer = new CookieContainer();

            //    var handler = new HttpClientHandler
            //    {
            //        CookieContainer = cookieContainer,
            //        UseCookies = true,
            //    };

            //    return handler;
            //})
            .AddStandardResilienceHandler();

        services.AddSingleton<StratzImporter>();
        services.AddSingleton<PlayerMatchImporter>();

        return services;
    }
}