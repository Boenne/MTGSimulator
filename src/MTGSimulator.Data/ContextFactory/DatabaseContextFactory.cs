using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MTGSimulator.Data.Contexts;

namespace MTGSimulator.Data.ContextFactory
{
    public interface IDatabaseContextFactory
    {
        DatabaseContext Create();
    }

    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        public DatabaseContext Create()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            dbContextOptionsBuilder.UseSqlServer(builder.GetConnectionString("DefaultConnectionString"));

            return new DatabaseContext(dbContextOptionsBuilder.Options);
        }
    }
}