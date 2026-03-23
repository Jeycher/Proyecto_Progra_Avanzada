using System.ComponentModel.DataAnnotations;

namespace TurismoRural_API.Models
{
    public class ComunidadCreateRequest
    {
        [Required]
        public string Nombre_Comunidad { get; set; } = string.Empty;

        [Required]
        public string Pais { get; set; } = string.Empty;

        public string? Provincia { get; set; }

        public string? Descripcion { get; set; }
    }
}
