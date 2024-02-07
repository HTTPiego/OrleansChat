using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Client.Repositories.Interfaces;
using Client.Repositories;
using Orleans.Hosting;
using Newtonsoft;
using Orleans.Serialization;
using Orleans.Storage;
using Orleans.Runtime;
using Orleans.Providers;
using Orleans.Core;

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

builder.Services.AddDbContext<ChatDbContext>(options =>
{
    //options.UseSqlServer(builder.Configuration["ConnectionStrings:Chat"]);
    //options.UseSqlServer(builder.Configuration.GetConnectionString("Chat"));
    options.UseSqlServer("Server=localhost;Database=OrleansChat;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
//builder.Services.AddScoped<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));

/*builder.Services.AddSerializer(serializerBuilder =>
{
    serializerBuilder.AddNewtonsoftJsonSerializer(
        isSupported: type => type.Namespace!.StartsWith("Grains.GrainState"));
});

builder.Services.AddSerializer(serializerBuilder =>
{
    serializerBuilder.AddJsonSerializer(
        isSupported: type => type.Namespace.StartsWith("Example.Namespace"));
});*/

/*builder.Services.AddSerializer(serializerBuilder =>
{
    serializerBuilder.AddJsonSerializer(
        isSupported: type => type.Namespace!.StartsWith("Grains.GrainState"));
});*/

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
        siloBuilder.AddMemoryStreams("chat", conf => conf.ConfigureStreamPubSub(Orleans.Streams.StreamPubSubType.ImplicitOnly));
        
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

//siloBuilder.AddMemoryStreams("chat");
//Zuercher.Orleans.Persistence.Redis

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();

//LAST VERSION