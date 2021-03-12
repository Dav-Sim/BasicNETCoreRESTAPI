using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Entities
{
    public class Detail
    {
        [Required]
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Text { get; set; }
        public Entities.Task Task { get; set; }
    }
}
