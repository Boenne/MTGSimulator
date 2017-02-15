using System.ComponentModel.DataAnnotations;

namespace MTGSimulator.Data.Models
{
    public class DraftPlayer
    {
        [Key]
        public string Id { get; set; }

        public int Number { get; set; }
        public string DraftSessionId { get; set; }
    }
}