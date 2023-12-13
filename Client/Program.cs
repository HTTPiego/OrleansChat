using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GrainInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

/*IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        client.AddMemoryStreams("chat")
              .UseLocalhostClustering();
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using IHost host = builder.Build();
await host.RunAsync();

IClusterClient client = host.Services.GetRequiredService<IClusterClient>();*/

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", b =>
{
    b
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200");
}));


builder.Host
    .UseOrleansClient(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryStreams("chat");
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();