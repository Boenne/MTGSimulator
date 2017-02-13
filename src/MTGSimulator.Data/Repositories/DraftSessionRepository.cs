using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MTGSimulator.Data.ContextFactory;
using MTGSimulator.Data.Models;
using Newtonsoft.Json;

namespace MTGSimulator.Data.Repositories
{
    public interface IDraftSessionRepository
    {
        Task Save(DraftSession draftSession);
        Task Start(string draftId);
        Task<DraftSession> Get(string draftId);
        Task Delete(string draftId);
        Task<bool> HasStarted(string draftId);
    }

    public class DraftSessionRepository : IDraftSessionRepository
    {
        private readonly IDatabaseContextFactory databaseContextFactory;
        private readonly ILogger logger;

        public DraftSessionRepository(IDatabaseContextFactory databaseContextFactory, ILogger logger)
        {
            this.databaseContextFactory = databaseContextFactory;
            this.logger = logger;
        }

        public async Task Save(DraftSession draftSession)
        {
            try
            {
                using (var databaseContext = databaseContextFactory.Create())
                {
                    databaseContext.DraftSessions.Add(draftSession);
                    await databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Save)} failed for session '{JsonConvert.SerializeObject(draftSession)}'", e);
            }
        }

        public async Task Start(string draftId)
        {
            try
            {
                using (var databaseContext = databaseContextFactory.Create())
                {
                    var draftSessionToUpdate =
                        databaseContext.DraftSessions.FirstOrDefault(x => x.Id == draftId);
                    if (draftSessionToUpdate == null) return;
                    draftSessionToUpdate.HasStarted = true;
                    await databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Start)} failed for draftId '{draftId}'", e);
            }
        }

        public async Task<DraftSession> Get(string draftId)
        {
            try
            {
                using (var databaseContext = databaseContextFactory.Create())
                {
                    var draftSession =
                        await databaseContext.DraftSessions.FirstOrDefaultAsync(x => x.Id == draftId);
                    return draftSession;
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Get)} failed for draftId '{draftId}'", e);
                return null;
            }
        }

        public async Task Delete(string draftId)
        {
            try
            {
                using (var databaseContext = databaseContextFactory.Create())
                {
                    var draftSession = databaseContext.DraftSessions.FirstOrDefault(x => x.Id == draftId);
                    if (draftSession == null) return;
                    databaseContext.DraftSessions.Remove(draftSession);
                    await databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Delete)} failed for draftId '{draftId}'", e);
            }
        }

        public async Task<bool> HasStarted(string draftId)
        {
            try
            {
                using (var databaseContext = databaseContextFactory.Create())
                {
                    var hasStarted = await databaseContext.DraftSessions.AnyAsync(x => x.Id == draftId && x.HasStarted);
                    return hasStarted;
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Delete)} failed for draftId '{draftId}'", e);
                return false;
            }
        }
    }
}