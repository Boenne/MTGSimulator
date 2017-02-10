using System.Collections.Generic;

namespace MTGSimulator.Service.Models
{
    public class Booster
    {
        public Booster()
        {
            Cards = new List<Card>();
        }

        public List<Card> Cards { get; set; }
    }
}