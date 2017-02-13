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
    public class DraftSessionRepositoryTests
    {
        public DraftSessionRepositoryTests()
        {
            var logger = new Mock<ILogger>();
            var databaseContextFactory = new DatabaseContextFactory();
            draftSessionRepository = new DraftSessionRepository(databaseContextFactory, logger.Object);
        }

        private readonly DraftSessionRepository draftSessionRepository;

        [Fact]
        public async Task AllMethodsWork()
        {
            var id = Guid.NewGuid().ToString().GenerateHash();
            var session = new DraftSession {Id = id, HasStarted = false};

            await draftSessionRepository.Save(session);
            var draftSession = await draftSessionRepository.Get(id);

            draftSession.Id.ShouldBe(id);

            var hasStarted = await draftSessionRepository.HasStarted(draftSession.Id);
            hasStarted.ShouldBe(false);

            await draftSessionRepository.Start(draftSession.Id);

            hasStarted = await draftSessionRepository.HasStarted(draftSession.Id);
            hasStarted.ShouldBe(true);

            draftSession = await draftSessionRepository.Get(id);
            draftSession.ShouldNotBeNull();

            await draftSessionRepository.Delete(id);
            draftSession = await draftSessionRepository.Get(id);

            draftSession.ShouldBeNull();
        }
    }
}