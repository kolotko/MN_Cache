using Distributed;
using Distributed.Abstractions;
using Distributed.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateHostBuilder(args).Build();
await host.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((builder, serviceBuilder) =>
        {
            serviceBuilder.AddHostedService<DistributedCache>();
            
            serviceBuilder.AddStackExchangeRedisCache(redisOptions =>
            {
                string connection = builder.Configuration.GetConnectionString("Redis");
                redisOptions.Configuration = connection;
            });


            serviceBuilder.AddSingleton<LongCalcSimulationService>();
            serviceBuilder.AddSingleton<ILongCalcSimulationService, LongCalcSimulationServiceWithCache>();

        });