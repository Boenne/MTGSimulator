using System;
using System.ComponentModel.DataAnnotations;

namespace MTGSimulator.Data.Models
{
    public class DraftSession
    {
        public DraftSession()
        {
            CreationTime = DateTime.Now;
        }

        [Key]
        public string Id { get; set; }

        public bool HasStarted { get; set; }
        public string SetCode { get; set; }
        public DateTime CreationTime { get; set; }
    }
}