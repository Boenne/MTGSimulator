using System.Linq;
using System.Threading.Tasks;
using MTGSimulator.Data.Contexts;
using MTGSimulator.Data.Models;

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
        public async Task Save(DraftSession draftSession)
        {
            using (var draftSessionContext = new DraftSessionContext())
            {
                draftSessionContext.DraftSessions.Add(draftSession);
                await draftSessionContext.SaveChangesAsync();
            }
        }

        public async Task Update(DraftSession draftSession)
        {
            using (var draftSessionContext = new DraftSessionContext())
            {
                var draftSessionToUpdate =
                    draftSessionContext.DraftSessions.FirstOrDefault(x => x.DraftId == draftSession.DraftId);
                if (draftSessionToUpdate == null) return;
                draftSessionToUpdate.HasStarted = draftSession.HasStarted;
                await draftSessionContext.SaveChangesAsync();
            }
        }

        public async Task<DraftSession> Get(string draftId)
        {
            using (var draftSessionContext = new DraftSessionContext())
            {
                var draftSession = draftSessionContext.DraftSessions.FirstOrDefault(x => x.DraftId == draftId);
                return draftSession;
            }
        }

        public async Task Delete(string draftId)
        {
            using (var draftSessionContext = new DraftSessionContext())
            {
                var draftSession = draftSessionContext.DraftSessions.FirstOrDefault(x => x.DraftId == draftId);
                if (draftSession == null) return;
                draftSessionContext.DraftSessions.Remove(draftSession);
                await draftSessionContext.SaveChangesAsync();
            }
        }
    }
}