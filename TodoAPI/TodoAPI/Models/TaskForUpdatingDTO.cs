using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class TaskForUpdatingDTO : TaskForManipulationDTO
    {
        [Required(ErrorMessage = "Description is required when updating Task")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
