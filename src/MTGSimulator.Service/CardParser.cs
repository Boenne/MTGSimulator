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
        public async Task<Dictionary<string, Set>> GetSets()
        {
            using (
                var resource =
                    ExecutingAssembly.Assembly.GetManifestResourceStream(
                        $"{ExecutingAssembly.Namespace}.Resources.AllSets.json"))
            {
                using (var sr = new StreamReader(resource))
                {
                    var json = await sr.ReadToEndAsync();
                    var sets = JsonConvert.DeserializeObject<Dictionary<string, Set>>(json);
                    return sets;
                }
            }
        }

        public async Task<Set> GetSet(string setCode)
        {
            var dictionary = await GetSets();
            if (dictionary.TryGetValue(setCode, out Set set))
                return set;
            throw new ArgumentException();
        }
    }
}