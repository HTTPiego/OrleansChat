using GrainInterfaces;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace Client.SignalR
{
    public sealed class ChatHub : Hub<IChatClient>
    {
        public Task JoinRoom(string roomName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        /*public async Task SendMessage(UserMessage message)
        {
            await Clients.Group(message.ChatRoomName).ReveiceMessage(message);
        }*/


    }
}