using System.ComponentModel.DataAnnotations;

namespace TurismoRural_API.Models
{
    public class ExperienciaConcurrenciaCreateRequest
    {
        [Required]
        public DateTime Fecha { get; set; }

        public string? Detalle { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Cupos_Disponibles { get; set; }

        [Required]
        public int ID_Experiencia { get; set; }
    }
}
