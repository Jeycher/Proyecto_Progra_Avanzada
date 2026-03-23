using System.ComponentModel.DataAnnotations;

namespace TurismoRural_API.Models
{
    public class SeguridadRequest
    {
        [Required]
        [MinLength(8)]
        public string NuevaContrasenna { get; set; } = string.Empty;
        [Required]
        [Compare("NuevaContrasenna")]
        public string ConfirmarContrasenna { get; set; } = string.Empty;
    }
}