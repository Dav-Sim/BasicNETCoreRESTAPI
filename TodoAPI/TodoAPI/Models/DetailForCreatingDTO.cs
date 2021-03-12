using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Models
{
    public class DetailForCreatingDTO : DetailForManipulationDTO
    {
    }
    public class DetailForUpdatingDTO : DetailForManipulationDTO
    {
        [Required(ErrorMessage = "You must provide detail text, when updating detail.")]
        public override string Text { get => base.Text; set => base.Text = value; }
    }
}
