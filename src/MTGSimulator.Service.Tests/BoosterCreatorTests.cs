using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace MTGSimulator.Service.Tests
{
    public class BoosterCreatorTests
    {
        [Fact]
        public async Task CreateBoosters_CreatesBoostersCorrectly()
        {
            var boosterCreator = new BoosterCreator(new CardParser());

            var boosters = await boosterCreator.CreateBoosters("AER", 3);

            boosters.Count.ShouldBe(3);
            boosters.ShouldAllBe(x => x.Cards.Count == 14);
            boosters.First().Cards.Count(x => x.Rarity == "Common").ShouldBe(10);
            boosters.First().Cards.Count(x => x.Rarity == "Uncommon").ShouldBe(3);
            boosters.First().Cards.Count(x => x.Rarity == "Rare" || x.Rarity == "Mythic Rare").ShouldBe(1);
        }
    }
}
