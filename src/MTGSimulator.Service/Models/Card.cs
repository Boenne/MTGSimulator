using Newtonsoft.Json;

namespace MTGSimulator.Service.Models
{
    public class Card
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("names")]
        public string[] Names { get; set; }

        [JsonProperty("manaCost")]
        public string ManaCost { get; set; }

        [JsonProperty("cmc")]
        public string ConvertedManaCost { get; set; }

        [JsonProperty("colors")]
        public string[] Colors { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("supertypes")]
        public string[] Supertypes { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }

        [JsonProperty("subtypes")]
        public string[] Subtypes { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("power")]
        public string Power { get; set; }

        [JsonProperty("loyalty")]
        public string Loyalty { get; set; }

        [JsonProperty("toughness")]
        public string Toughness { get; set; }

        [JsonProperty("multiverseid")]
        public string MultiverseId { get; set; }
    }
}