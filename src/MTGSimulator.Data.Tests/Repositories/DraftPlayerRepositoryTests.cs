using System;
using System.Threading.Tasks;
using Moq;
using MTGSimulator.Data.ContextFactory;
using MTGSimulator.Data.Extensions;
using MTGSimulator.Data.Models;
using MTGSimulator.Data.Repositories;
using Shouldly;
using Xunit;

namespace MTGSimulator.Data.Tests.Repositories
{
    public class DraftPlayerRepositoryTests
    {
        public DraftPlayerRepositoryTests()
        {
            var logger = new Mock<ILogger>();
            var databaseContextFactory = new DatabaseContextFactory();
            draftPlayerRepository = new DraftPlayerRepository(databaseContextFactory, logger.Object);
            draftSessionRepository = new DraftSessionRepository(databaseContextFactory, logger.Object);
        }

        private readonly DraftPlayerRepository draftPlayerRepository;
        private readonly DraftSessionRepository draftSessionRepository;

        [Fact]
        public async Task AllMethodsWork()
        {
            var sessionId = Guid.NewGuid().ToString().GenerateHash();
            var playerId = Guid.NewGuid().ToString().GenerateHash();
            var session = new DraftSession {Id = sessionId, HasStarted = true};
            var draftPlayer = new DraftPlayer {DraftSessionId = session.Id, Id = playerId};

            await draftSessionRepository.Save(session);
            await draftPlayerRepository.Save(draftPlayer);

            var playerExists = await draftPlayerRepository.PlayerExists(playerId, draftPlayer.Id);
            playerExists.ShouldBe(true);

            await draftSessionRepository.Delete(session.Id);
            await draftPlayerRepository.Delete(playerId);

            playerExists = await draftPlayerRepository.PlayerExists(playerId, draftPlayer.Id);
            playerExists.ShouldBe(false);
        }
    }
}