using System;
using Newtonsoft.Json;

namespace MTGSimulator.Service.Models
{
    public class Set
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("gathererCode")]
        public string GathererCode { get; set; }

        [JsonProperty("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("block")]
        public string Block { get; set; }

        [JsonProperty("cards")]
        public Card[] Cards { get; set; }
    }
}