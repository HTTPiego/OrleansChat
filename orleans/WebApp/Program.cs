using WebApp;
using WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.UseSignalR();
    siloBuilder.RegisterHub<ChatRoomHub>();
});

builder.Services.AddControllers();
builder.Services
    .AddSignalR()
    .AddOrleans();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<ChatRoomHub>("chat-room-hub");

app.Run();
