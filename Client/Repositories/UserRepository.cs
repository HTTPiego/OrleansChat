﻿using Client.Repositories.Interfaces;
using Grains.GrainState;
using Microsoft.EntityFrameworkCore;

namespace Client.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly ChatDbContext _context;

        private bool _disposed;

        public UserRepository(ChatDbContext context)
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

        public async Task<List<ChatRoomState>> GetAllUsers()
        {
            return await _context.Chats.ToListAsync();
        }

        public async Task<UserState> AddUser(UserState user)
        {
            if (user != null)
                throw new ArgumentNullException(nameof(user));
            _context.Users.Add(user!);
            return await Task.FromResult(user!);
        }

        public async Task<UserState> RemoveUser(UserState user)
        {
            if (user != null)
                throw new ArgumentNullException(nameof(user));
            _context.Users.Remove(user!);
            return await Task.FromResult(user!);
        }

    }
}
