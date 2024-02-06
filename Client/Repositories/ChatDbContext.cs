using Grains;
using Grains.GrainState;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ChatDbContext : DbContext
    {
        public DbSet<UserDB> Users { get; set; }

        public DbSet<ChatRoomDB> Chats { get; set; }

        public ChatDbContext() { }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Chat"));
        }*/
    }
}
