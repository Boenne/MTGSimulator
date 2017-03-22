using System;
using System.Threading.Tasks;
using Moq;
using MTGSimulator.Data.Cache;
using MTGSimulator.Data.ContextFactory;
using MTGSimulator.Data.Extensions;
using MTGSimulator.Data.Models;
using MTGSimulator.Data.Repositories;
using Shouldly;
using Xunit;

namespace MTGSimulator.Data.Tests.Repositories
{
    public class DraftSessionRepositoryTests
    {
        public DraftSessionRepositoryTests()
        {
            var logger = new Mock<ILogger>();
            var databaseContextFactory = new DatabaseContextFactory("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True");
            var cacheService = new Mock<ICacheService>();
            draftSessionRepository = new DraftSessionRepository(databaseContextFactory, logger.Object, cacheService.Object);
        }

        private readonly DraftSessionRepository draftSessionRepository;

        [Fact]
        public async Task AllMethodsWork()
        {
            var sessionId = Guid.NewGuid().ToString().GenerateHash();
            var session = new DraftSession {Id = sessionId, HasStarted = false, SetCode = "LEA"};

            await draftSessionRepository.Save(session);

            var hasStarted = await draftSessionRepository.HasStarted(sessionId);
            hasStarted.ShouldBe(false);

            await draftSessionRepository.Start(sessionId);

            hasStarted = await draftSessionRepository.HasStarted(sessionId);
            hasStarted.ShouldBe(true);

            var draftSession = await draftSessionRepository.Get(sessionId);
            draftSession.ShouldNotBeNull();

            await draftSessionRepository.Delete(sessionId);
            draftSession = await draftSessionRepository.Get(sessionId);

            draftSession.ShouldBeNull();
        }
    }
}