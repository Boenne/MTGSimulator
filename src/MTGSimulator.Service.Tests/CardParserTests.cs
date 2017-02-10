using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace MTGSimulator.Service.Tests
{
    public class CardParserTests
    {
        [Fact]
        public async Task GetSet_ReturnsCorrectSet()
        {
            var cardParser = new CardParser();

            var set = await cardParser.GetSet("LEA");

            set.Cards.Count.ShouldBe(295);
        }

        [Fact]
        public async Task GetSets_ReturnsDictionary()
        {
            var cardParser = new CardParser();

            var sets = await cardParser.GetSets();

            sets.ShouldNotBeNull();
        }
    }
}