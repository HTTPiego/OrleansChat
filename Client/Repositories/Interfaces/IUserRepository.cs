using Grains.GrainState;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories.Interfaces
{
    public interface IUserRepository : IDisposable
    {
        Task<List<UserState>> GetAllUsers();

        Task<List<UserState>> GetAllUsersBySubstring(string substring);

        Task<UserState> UserIsRegistered(string username);

        Task<UserState> AddUser(UserState user);

        Task<UserState> RemoveUser(UserState user);

    }
}
