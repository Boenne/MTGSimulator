using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MTGSimulator.Service.Models;
using Newtonsoft.Json;

namespace MTGSimulator.Service
{
    public interface ICardParser
    {
        Task<Dictionary<string, Set>> GetSets();
        Task<Set> GetSet(string setCode);
    }

    public class CardParser : ICardParser
    {
        public Task<Dictionary<string, Set>> GetSets()
        {
            return Task.Run(() =>
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var json = File.ReadAllText($"{currentDirectory}../Resources/AllSets.json");
                var sets = JsonConvert.DeserializeObject<Dictionary<string, Set>>(json);
                return sets;
            });
        }

        public async Task<Set> GetSet(string setCode)
        {
            var dictionary = await GetSets();
            Set set;
            if (dictionary.TryGetValue(setCode, out set))
                return set;
            throw new ArgumentException();
        }
    }
}