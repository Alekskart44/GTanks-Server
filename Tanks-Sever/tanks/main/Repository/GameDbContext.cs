using Tanks_Sever.tanks.users;

using Microsoft.EntityFrameworkCore;
using Tanks_Sever.tanks.users.garage;

namespace Repository
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Garage> Garages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
