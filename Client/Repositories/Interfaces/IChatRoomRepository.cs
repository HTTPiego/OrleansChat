using Grains;
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
        Task<List<ChatRoomDB>> GetAllChatRooms();

        Task<ChatRoomDB?> GetChatRoomBy(string chatname);

        ChatRoomDB AddChatRoom(ChatRoomDB chat);

        Task<ChatRoomDB> RemoveChatRoom(ChatRoomDB chat);
    }
}
