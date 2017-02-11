using System;
using System.Threading.Tasks;
using Moq;
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
            draftSessionRepository = new DraftSessionRepository(logger.Object);
        }

        private readonly DraftSessionRepository draftSessionRepository;

        [Fact]
        public async Task AllMethodsWork()
        {
            var id = Guid.NewGuid().ToString().GenerateHash();
            var session = new DraftSession {DraftId = id, HasStarted = true};

            await draftSessionRepository.Save(session);
            var draftSession = await draftSessionRepository.Get(id);

            draftSession.DraftId.ShouldBe(id);
            draftSession.HasStarted.ShouldBe(true);

            draftSession.HasStarted = false;
            await draftSessionRepository.Update(draftSession);

            draftSession = await draftSessionRepository.Get(id);
            draftSession.HasStarted.ShouldBe(false);

            await draftSessionRepository.Delete(id);
            draftSession = await draftSessionRepository.Get(id);

            draftSession.ShouldBeNull();
        }
    }
}