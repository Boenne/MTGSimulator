using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTGSimulator.Data.Extensions;
using MTGSimulator.Service.Models;
using Newtonsoft.Json;

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

        private Booster CreateBooster(IReadOnlyList<Card> commons, IReadOnlyList<Card> unCommons,
            IReadOnlyList<Card> rares)
        {
            var booster = new Booster();
            var random = new Random();
            for (var i = 0; i < 14; i++)
            {
                if (i < 10)
                {
                    var index = random.Next(commons.Count);
                    booster.Cards.Add(CopyCard(commons[index]));
                }
                else if (i < 13)
                {
                    var index = random.Next(unCommons.Count);
                    booster.Cards.Add(CopyCard(unCommons[index]));
                }
                else
                {
                    var index = random.Next(rares.Count);
                    booster.Cards.Add(CopyCard(rares[index]));
                }
            }
            //Give each card a unique id
            booster.Cards.ForEach(x => x.Id = Guid.NewGuid().ToString());
            return booster;
        }

        private Card CopyCard(Card card)
        {
            var json = JsonConvert.SerializeObject(card);
            return JsonConvert.DeserializeObject<Card>(json);
        }
    }
}