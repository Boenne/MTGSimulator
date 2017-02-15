using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTGSimulator.Service.Models;

namespace MTGSimulator.Service
{
    public interface IBoosterCreator
    {
        Task<List<Booster>> CreateBoosters(string setCode, int numberOfBoosters);
    }

    public class BoosterCreator : IBoosterCreator
    {
        private readonly ICardParser cardParser;

        public BoosterCreator(ICardParser cardParser)
        {
            this.cardParser = cardParser;
        }

        public async Task<List<Booster>> CreateBoosters(string setCode, int numberOfBoosters)
        {
            var set = await cardParser.GetSet(setCode);
            var commons = set.Cards.Where(x => x.Rarity == "Common").ToList();
            var unCommons = set.Cards.Where(x => x.Rarity == "Uncommon").ToList();
            var rares = set.Cards.Where(x => x.Rarity == "Rare" || x.Rarity == "Mythic Rare").ToList();

            var boosters = new List<Booster>();
            for (var i = 0; i < numberOfBoosters; i++)
            {
                var booster = CreateBooster(commons, unCommons, rares);
                boosters.Add(booster);
            }
            return boosters;
        }

        private static Booster CreateBooster(IReadOnlyList<Card> commons, IReadOnlyList<Card> unCommons,
            IReadOnlyList<Card> rares)
        {
            var booster = new Booster();
            for (var j = 0; j < 14; j++)
            {
                if (j < 10)
                {
                    var index = new Random().Next(commons.Count);
                    booster.Cards.Add(commons[index]);
                }
                else if (j < 13)
                {
                    var index = new Random().Next(unCommons.Count);
                    booster.Cards.Add(unCommons[index]);
                }
                else
                {
                    var index = new Random().Next(rares.Count);
                    booster.Cards.Add(rares[index]);
                }
            }
            return booster;
        }
    }
}