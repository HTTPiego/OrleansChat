using Grains.GrainState;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ChatDbContext : DbContext
    {
        public DbSet<UserState> Users { get; set; }

        public DbSet<ChatRoomState> Chats { get; set; }

        public ChatDbContext() { }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }
    }
}
