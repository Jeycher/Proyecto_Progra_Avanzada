using System.ComponentModel.DataAnnotations;

namespace TurismoRural_WEB.Models
{
    public class UserEditViewModel
    {
        public int id_Usuario { get; set; }

        [Required]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string correo { get; set; } = string.Empty;

        [Required]
        public string telefono { get; set; } = string.Empty;

        [Required]
        public int id_Rol { get; set; }
    }
}
