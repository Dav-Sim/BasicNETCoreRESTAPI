using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.ValidationAttributes;

namespace TodoAPI.Models
{
    [TaskNameAndDescCannotBeSame(ErrorMessage = "Description and Name must have different values")]
    public abstract class TaskForManipulationDTO
    {
        [Required]
        [MaxLength(100)]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        [Range(0, double.MaxValue)]
        public virtual int Priority { get; set; }
        public virtual string Status { get; set; }
        public virtual List<DetailForCreatingDTO> Details { get; set; } = new List<DetailForCreatingDTO>();
    }
}
