using Microsoft.EntityFrameworkCore;
using MTGSimulator.Data.Contexts;

namespace MTGSimulator.Data.ContextFactory
{
    public interface IDatabaseContextFactory
    {
        DatabaseContext Create();
    }

    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly string connectionString;

        public DatabaseContextFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DatabaseContext Create()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            dbContextOptionsBuilder.UseSqlServer(connectionString);

            return new DatabaseContext(dbContextOptionsBuilder.Options);
        }
    }
}