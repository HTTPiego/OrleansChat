using Microsoft.AspNetCore.SignalR;

namespace WebApp.Hubs;

public class ChatRoomHub: Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}");
    }
}