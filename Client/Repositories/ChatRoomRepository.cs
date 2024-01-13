using Client.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task<>

        //TODO: Implement

    }
}
