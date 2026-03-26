using System.ComponentModel.DataAnnotations;

namespace TurismoRural_API.Models
{
    public class RegistrarUsuarioRequest
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; } = string.Empty;
        [Required]
        public string Contrasenna { get; set; } = string.Empty;

        public string? Telefono { get; set; }
    }
}