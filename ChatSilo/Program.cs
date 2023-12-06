using GrainInterfaces;
using Grains;
using Orleans.Runtime;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryStreams("chat");
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

var host = builder.Build();
await host.RunAsync();
