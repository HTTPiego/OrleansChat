using Microsoft.AspNetCore.SignalR;

namespace Client.SignalR
{
    public sealed class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
        }

        public async Task Ciao()
        {
            await Task.CompletedTask;
        }
    }
}