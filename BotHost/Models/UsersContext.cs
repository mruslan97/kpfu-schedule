using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BotHost.Models
{
    public class UsersContext : DbContext
    {
        public DbSet<TgUser> TgUsers { get; set; }
        public DbSet<VkUser> VkUsers { get; set; }
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public UsersContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString =
                "Host=localhost;Port=5432;Database=KfuScheduleDb;Username=postgres;Password=postgres";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
