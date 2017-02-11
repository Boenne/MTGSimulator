using System.Data.Entity;
using MTGSimulator.Data.Models;

namespace MTGSimulator.Data.Contexts
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True")
        {
        }


        public DbSet<DraftSession> DraftSessions { get; set; }
        public DbSet<DraftPlayer> DraftPlayers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DraftSession>().ToTable("DraftSessionsV2");
        }
    }
}