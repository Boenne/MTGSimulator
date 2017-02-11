using Microsoft.EntityFrameworkCore;
using MTGSimulator.Data.Models;

namespace MTGSimulator.Data.Contexts
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }


        public DbSet<DraftSession> DraftSessions { get; set; }
        public DbSet<DraftPlayer> DraftPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DraftSession>().ToTable("DraftSessionsV2");
        }
    }
}