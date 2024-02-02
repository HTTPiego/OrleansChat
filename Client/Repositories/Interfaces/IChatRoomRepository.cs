using Grains.GrainState;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories.Interfaces
{
    public interface IChatRoomRepository : IDisposable
    {
        Task<List<ChatRoomState>> GetAllChatRooms();

        Task<ChatRoomState?> GetChatRoomBy(string chatname);

        Task<ChatRoomState> AddChatRoom(ChatRoomState chat);

        Task<ChatRoomState> RemoveChatRoom(ChatRoomState chat);
    }
}
