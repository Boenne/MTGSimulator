using System;
using System.ComponentModel.DataAnnotations;

namespace MTGSimulator.Data.Models
{
    public class DraftPlayer
    {
        public DraftPlayer()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string PlayerId { get; set; }
        public Guid DraftSessionId { get; set; }
    }
}