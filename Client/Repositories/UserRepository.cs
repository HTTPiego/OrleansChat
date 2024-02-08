using Client.Repositories.Interfaces;
using Grains;
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

        public async Task<List<UserDB>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<UserDB>> GetAllUsersBySubstring(string substring)
        {
            return await _context.Users.Where(user => user.Username.Contains(substring.ToLower())).ToListAsync();
        }

        public async Task<UserDB> UserIsRegistered(string username)
        {
            return await Task.FromResult(_context.Users.Where(user => user.Username.Equals(username)).ToList().FirstOrDefault()!);
        }

        public UserDB AddUser(UserDB user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (_context.Users.Contains(user))
                throw new ArgumentException("The user " + user.Username + " already exists");
            var response = _context.Users.Add(user!);
            _context.SaveChanges();
            return response.Entity;
        }

        public async Task<UserDB> RemoveUser(UserDB user)
        {
            if (user != null)
                throw new ArgumentNullException(nameof(user));
            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();
            return await Task.FromResult(user!);
        }

    }
}
