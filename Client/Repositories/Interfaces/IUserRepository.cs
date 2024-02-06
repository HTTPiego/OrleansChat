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
    public interface IUserRepository : IDisposable
    {
        Task<List<UserDB>> GetAllUsers();

        Task<List<UserDB>> GetAllUsersBySubstring(string substring);

        Task<UserDB> UserIsRegistered(string username);

        UserDB AddUser(UserDB user);

        Task<UserDB> RemoveUser(UserDB user);

    }
}
