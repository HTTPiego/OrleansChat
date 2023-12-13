using GrainInterfaces;
using Grains;
using Orleans.Runtime;
using StackExchange.Redis;


IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        //siloBuilder.AddMemoryStreams("chat");
       siloBuilder.AddRedisGrainStorageAsDefault(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions();
            options.ConfigurationOptions.EndPoints.Add("localhost", 6379);
        });
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

var host = builder.Build();
await host.RunAsync();
