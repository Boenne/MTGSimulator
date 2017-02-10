using System.Data.Entity;
using MTGSimulator.Data.Models;

namespace MTGSimulator.Data.Contexts
{
    public class DraftSessionContext : DbContext
    {
        public DraftSessionContext()
            : base("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True")
        {
        }


        public DbSet<DraftSession> DraftSessions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DraftSession>().ToTable("DraftSessionsV2");
        }
    }
}