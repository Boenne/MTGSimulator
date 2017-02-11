﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MTGSimulator.Data.Contexts;
using MTGSimulator.Data.Models;
using Newtonsoft.Json;

namespace MTGSimulator.Data.Repositories
{
    public interface IDraftSessionRepository
    {
        Task Save(DraftSession draftSession);
        Task Update(DraftSession draftSession);
        Task<DraftSession> Get(string draftId);
        Task Delete(string draftId);
    }

    public class DraftSessionRepository : IDraftSessionRepository
    {
        private readonly ILogger logger;

        public DraftSessionRepository(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task Save(DraftSession draftSession)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
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

        public async Task Update(DraftSession draftSession)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var draftSessionToUpdate =
                        databaseContext.DraftSessions.FirstOrDefault(x => x.DraftId == draftSession.DraftId);
                    if (draftSessionToUpdate == null) return;
                    draftSessionToUpdate.HasStarted = draftSession.HasStarted;
                    await databaseContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Update)} failed for session '{JsonConvert.SerializeObject(draftSession)}'", e);
            }
        }

        public async Task<DraftSession> Get(string draftId)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var draftSession =
                        await databaseContext.DraftSessions.FirstOrDefaultAsync(x => x.DraftId == draftId);
                    return draftSession;
                }
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(Update)} failed for draftId '{draftId}'", e);
                return null;
            }
        }

        public async Task Delete(string draftId)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var draftSession = databaseContext.DraftSessions.FirstOrDefault(x => x.DraftId == draftId);
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
    }
}