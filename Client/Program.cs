using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Client.Repositories.Interfaces;
using Client.Repositories;
using Client.SignalR;

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

builder.Services.AddControllers(); //AddControllersWithViews
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

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

/*app.UseEndpoints(endpoints =>
{
    app.MapHub<ChatHub>("send-message");
});*/

app.MapHub<ChatHub>("send-message");

app.MapControllers();



app.Run();

//LAST VERSION