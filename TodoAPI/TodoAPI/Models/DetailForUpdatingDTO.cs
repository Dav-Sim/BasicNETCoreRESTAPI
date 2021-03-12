using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class DetailForUpdatingDTO : DetailForManipulationDTO
    {
        [Required(ErrorMessage = "You must provide detail text, when updating detail.")]
        public override string Text { get => base.Text; set => base.Text = value; }
    }
}
