using Client.Repositories.Interfaces;
using Grains;
using Grains.GrainState;
using Microsoft.EntityFrameworkCore;

namespace Client.Repositories
{
    public class ChatRoomRepository : IChatRoomRepository
    {

        private readonly ChatDbContext _context;

        private bool _disposed;

        public ChatRoomRepository(ChatDbContext context) 
        {
            _context = context;
            _disposed = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<List<ChatRoomState>> GetAllChatRooms()
        {
            return await _context.Chats.ToListAsync();
        }

        public async Task<ChatRoomState?> GetChatRoomBy(string chatname)
        {
            return await _context.Chats.Where(chat => chat.ChatName.Equals(chatname)).FirstOrDefaultAsync();
        }

        public async Task<ChatRoomState> AddChatRoom(ChatRoomState chat)
        {
            if (_context.Chats.Contains(chat))
            {
                throw new ArgumentException("The chat \"" + chat.ChatName + "\" already exists");
            }
            if(chat != null)
                throw new ArgumentNullException(nameof(chat));
            _context.Chats.Add(chat!);
            return await Task.FromResult(chat!);
        }

        public async Task<ChatRoomState> RemoveChatRoom(ChatRoomState chat)
        {
            if (chat != null)
                throw new ArgumentNullException(nameof(chat));
            _context.Chats.Remove(chat!);
            return await Task.FromResult(chat!);
        }

    }
}
