using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Models
{
    public abstract class DetailForManipulationDTO
    {
        [Required]
        [MaxLength(100)]
        public virtual string Title { get; set; }
        [MaxLength(1000)]
        public virtual string Text { get; set; }
    }
}
