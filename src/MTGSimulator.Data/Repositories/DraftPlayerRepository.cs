using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MTGSimulator.Data.Cache;
using MTGSimulator.Data.ContextFactory;
using MTGSimulator.Data.Models;
using Newtonsoft.Json;

namespace MTGSimulator.Data.Repositories
{
    public interface IDraftPlayerRepository
    {
        Task<bool> PlayerExists(string playerId, string draftId);
        Task Save(DraftPlayer draftPlayer);
        Task Delete(string playerId);
        Task<int> GetNumberOfPlayers(string draftId);
        Task<string> GetNextPlayer(string playerId, string draftId);
    }

    public class DraftPlayerRepository : IDraftPlayerRepository
    {
        private readonly IDatabaseContextFactory databaseContextFactory;
        private readonly ILogger logger;
        private readonly ICacheService cacheService;

        public DraftPlayerRepository(IDatabaseContextFactory databaseContextFactory, ILogger logger,
            ICacheService cacheService)
        {
            this.databaseContextFactory = databaseContextFactory;
            this.logger = logger;
            this.cacheService = cacheService;
        }

        public async Task<bool> PlayerExists(string playerId, string draftId)
        {
            try
            {
                var cacheServiceResponse = cacheService.Get<bool>(nameof(PlayerExists), playerId, draftId);
                if (cacheServiceResponse.Hit) return cacheServiceResponse.Value;

                using (var databaseContext = databaseContextFactory.Create())
                {
                    var playerExists =
                        await databaseContext.DraftPlayers.AnyAsync(x => x.Id == playerId && x.DraftSessionId == draftId);

                    cacheService.Cache(nameof(PlayerExists), draftId, playerExists, playerId, draftId);

                    return playerExists;
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(PlayerExists)} failed for playerId '{playerId}' and draftId '{draftId}'", e);
                return false;
            }
        }

        public async Task Save(DraftPlayer draftPlayer)
        {
            try
            {
                using (var databaseContext = databaseContextFactory.Create())
                {
                    var count =
                        await databaseContext.DraftPlayers.CountAsync(
                            x => x.DraftSessionId == draftPlayer.DraftSessionId);
                    draftPlayer.Number = count;
                    databaseContext.DraftPlayers.Add(draftPlayer);
                    await databaseContext.SaveChangesAsync();

                    cacheService.Invalidate(draftPlayer.DraftSessionId);
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Save)} failed for player '{JsonConvert.SerializeObject(draftPlayer)}'", e);
            }
        }

        public async Task<int> GetNumberOfPlayers(string draftId)
        {
            try
            {
                var cacheServiceResponse = cacheService.Get<int>(nameof(GetNumberOfPlayers), draftId);
                if (cacheServiceResponse.Hit) return cacheServiceResponse.Value;

                using (var databaseContext = databaseContextFactory.Create())
                {
                    var numberOfPlayers = await databaseContext.DraftPlayers.CountAsync(x => x.DraftSessionId == draftId);

                    cacheService.Cache(nameof(GetNumberOfPlayers), draftId, numberOfPlayers, draftId);

                    return numberOfPlayers;
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(GetNumberOfPlayers)} failed for draftId '{draftId}'", e);
                return 0;
            }
        }

        public async Task Delete(string playerId)
        {
            try
            {
                using (var databaseContext = databaseContextFactory.Create())
                {
                    var draftPlayer =
                        await databaseContext.DraftPlayers.FirstOrDefaultAsync(x => x.Id == playerId);
                    if (draftPlayer == null) return;
                    databaseContext.DraftPlayers.Remove(draftPlayer);
                    await databaseContext.SaveChangesAsync();

                    cacheService.Invalidate(draftPlayer.DraftSessionId);
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Delete)} failed for playerId '{playerId}'", e);
            }
        }

        public async Task<string> GetNextPlayer(string playerId, string draftId)
        {
            try
            {
                var cacheServiceResponse = cacheService.Get<string>(nameof(GetNextPlayer), playerId, draftId);
                if (cacheServiceResponse.Hit) return cacheServiceResponse.Value;

                using (var databaseContext = databaseContextFactory.Create())
                {
                    var numberOfPlayers = await databaseContext.DraftPlayers.CountAsync(x => x.DraftSessionId == draftId);
                    var player =
                        await databaseContext.DraftPlayers.FirstAsync(
                            x => x.Id == playerId && x.DraftSessionId == draftId);
                    DraftPlayer draftPlayer;
                    if (player.Number < numberOfPlayers - 1)
                        draftPlayer =
                            await databaseContext.DraftPlayers.FirstAsync(
                                x => x.Number == player.Number + 1 && x.DraftSessionId == draftId);
                    else
                        draftPlayer =
                            await databaseContext.DraftPlayers.FirstAsync(
                                x => x.Number == 0 && x.DraftSessionId == draftId);

                    cacheService.Cache(nameof(GetNextPlayer), draftId, draftPlayer.Id, playerId, draftId);

                    return draftPlayer.Id;
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(GetNextPlayer)} failed for playerId '{playerId}' and draftId '{draftId}'", e);
                return null;
            }
        }
    }
}