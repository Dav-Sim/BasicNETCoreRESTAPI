using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Entities
{
    public class Task
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, double.MaxValue)]
        public int Priority { get; set; }
        public Status Status { get; set; }
        [Required]
        public DateTime Updated { get; set; }
        public List<Detail> Details { get; set; } = new List<Detail>();
    }

    public enum Status
    {
        NotStarted,
        InProgress,
        Completed
    }
}
