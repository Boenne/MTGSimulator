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
            var databaseContextFactory = new DatabaseContextFactory("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True");
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
            var player2Id = Guid.NewGuid().ToString().GenerateHash();
            var session = new DraftSession {Id = sessionId, HasStarted = true, SetCode = "LEA"};
            var draftPlayer = new DraftPlayer {DraftSessionId = session.Id, Id = playerId};
            var draftPlayer2 = new DraftPlayer {DraftSessionId = session.Id, Id = player2Id};

            await draftSessionRepository.Save(session);
            await draftPlayerRepository.Save(draftPlayer);
            await draftPlayerRepository.Save(draftPlayer2);

            var playerExists = await draftPlayerRepository.PlayerExists(playerId, sessionId);
            playerExists.ShouldBe(true);

            var numberOfPlayers = await draftPlayerRepository.GetNumberOfPlayers(sessionId);
            numberOfPlayers.ShouldBe(2);

            var nextPlayerId = await draftPlayerRepository.GetNextPlayer(playerId, sessionId);
            nextPlayerId.ShouldBe(player2Id);

            nextPlayerId = await draftPlayerRepository.GetNextPlayer(player2Id, sessionId);
            nextPlayerId.ShouldBe(playerId);

            await draftPlayerRepository.Delete(playerId);
            await draftPlayerRepository.Delete(player2Id);
            await draftSessionRepository.Delete(sessionId);

            playerExists = await draftPlayerRepository.PlayerExists(playerId, draftPlayer.Id);
            playerExists.ShouldBe(false);
        }
    }
}