using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Client.Repositories.Interfaces;
using Client.Repositories;
using Client.SignalR;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseSqlServer("Server=localhost;User ID=SA;Password=DIEGOfranco2;Initial Catalog=OrleanChat;Integrated Security=False; TrustServerCertificate=True");
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
builder.Services.AddSingleton<ChatHub>();

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
        siloBuilder.UseSignalR(config:null);
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapHub<ChatHub>("/chat-hub");

app.MapControllers();

app.Run();

//LAST VERSION