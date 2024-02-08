using GrainInterfaces;
using Grains;
using Orleans.Runtime;
using StackExchange.Redis;
using Orleans.Serialization;
using Orleans.Providers;
using Orleans.Storage;
using Orleans.Persistence.Redis;
using Orleans.Core;


IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryStreams("chat", conf => conf.ConfigureStreamPubSub(Orleans.Streams.StreamPubSubType.ImplicitOnly));
        //siloBuilder.AddRedisStreams()
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
