using System;
using System.Data.Entity;
using System.Threading.Tasks;
using MTGSimulator.Data.Contexts;
using MTGSimulator.Data.Models;
using Newtonsoft.Json;

namespace MTGSimulator.Data.Repositories
{
    public interface IDraftPlayerRepository
    {
        Task<bool> PlayerExists(string playerId, Guid id);
        Task Save(DraftPlayer draftPlayer);
        Task Delete(string playerId);
    }

    public class DraftPlayerRepository : IDraftPlayerRepository
    {
        private readonly ILogger logger;

        public DraftPlayerRepository(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<bool> PlayerExists(string playerId, Guid id)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var playerExists =
                        await databaseContext.DraftPlayers.AnyAsync(x => x.PlayerId == playerId && x.Id == id);
                    return playerExists;
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(PlayerExists)} failed for playerId '{playerId}' and id '{id}'", e);
                return false;
            }
        }

        public async Task Save(DraftPlayer draftPlayer)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    databaseContext.DraftPlayers.Add(draftPlayer);
                    await databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Save)} failed for player '{JsonConvert.SerializeObject(draftPlayer)}'", e);
            }
        }

        public async Task Delete(string playerId)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var draftPlayer =
                        await databaseContext.DraftPlayers.FirstOrDefaultAsync(x => x.PlayerId == playerId);
                    if (draftPlayer == null) return;
                    databaseContext.DraftPlayers.Remove(draftPlayer);
                    await databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Delete)} failed for playerId '{playerId}'", e);
            }
        }
    }
}