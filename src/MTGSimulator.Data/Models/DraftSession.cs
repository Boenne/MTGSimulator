using System;
using System.ComponentModel.DataAnnotations;

namespace MTGSimulator.Data.Models
{
    public class DraftSession
    {
        public DraftSession()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public bool HasStarted { get; set; }
        public string DraftId { get; set; }
    }
}