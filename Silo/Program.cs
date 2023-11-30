using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleans(silo =>
    {
        silo.UseLocalhostClustering()
            .AddMemoryStreams("chat")
            .AddMemoryGrainStorage("PubSubStore")
            .ConfigureLogging(logging => logging.AddConsole());
    })
    .UseConsoleLifetime();


using IHost host = builder.Build();

await host.RunAsync();