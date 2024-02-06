using GrainInterfaces;
using Grains;
using Orleans.Runtime;
using StackExchange.Redis;
using Orleans.Serialization;
using Orleans.Providers;
using Orleans.Storage;
using Orleans.Persistence.Redis;


IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryStreams("chat");
        //siloBuilder.AddRedisStreams();
        siloBuilder.AddRedisGrainStorageAsDefault(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions();
            options.ConfigurationOptions.EndPoints.Add("localhost", 6379);
        });
        siloBuilder.Services.AddSerializer(serializerBuilder =>
        {
            serializerBuilder.AddNewtonsoftJsonSerializer(
                isSupported: type => type.Namespace!.StartsWith("Grains.GrainState"));
        });
        /*siloBuilder.Services.AddSerializer(serializerBuilder =>
        {
            serializerBuilder.AddJsonSerializer(
                isSupported: type => type.Namespace!.StartsWith("Grains.GrainState"));
        });*/
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

var host = builder.Build();
await host.RunAsync();
