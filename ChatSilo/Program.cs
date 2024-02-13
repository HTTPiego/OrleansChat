using SignalR.Orleans;
using StackExchange.Redis;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryStreams("chat", 
            conf => conf.ConfigureStreamPubSub(Orleans.Streams.StreamPubSubType.ImplicitOnly)) ;
        //siloBuilder.AddRedisStreams()
        siloBuilder.AddRedisGrainStorageAsDefault(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions();
            options.ConfigurationOptions.EndPoints.Add("localhost", 6379);
        });
        //siloBuilder.AddSomeOtherGrainStorage(SignalROrleansConstants.SIGNALR_ORLEANS_STORAGE_PROVIDER); AddMemoryGrainStorage

        siloBuilder.UseSignalR();
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

var host = builder.Build();
await host.RunAsync();
