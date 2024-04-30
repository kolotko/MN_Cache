using Microsoft.Extensions.Hosting;

var builder = CreateHostBuilder(args).Build();
await builder.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices(serviceBuilder =>
        {

            // serviceBuilder.AddHostedService<InMemoryCache>();
            // serviceBuilder.AddMemoryCache();
            //
            // serviceBuilder.AddSingleton<LongCalcSimulationService>();
            // serviceBuilder.AddSingleton<ILongCalcSimulationService>(x => new LongCalcSimulationServiceWithCache(x.GetService<LongCalcSimulationService>(), x.GetService<IMemoryCache>()));

        });