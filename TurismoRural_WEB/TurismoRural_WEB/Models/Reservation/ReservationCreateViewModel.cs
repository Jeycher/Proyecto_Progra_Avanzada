using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TurismoRural_WEB.Models.Reservation
{
    public class ConcurrenciaDto
    {
        public int ID_Concurrencia { get; set; }
        public string Detalle { get; set; }
        public int ID_Experiencia { get; set; }
        public string TituloExperiencia { get; set; }
    }

    public class ExperienciaDto
    {
        public int ID_Experiencia { get; set; }
        public string Titulo { get; set; }
    }

    public class ReservationCreateViewModel
    {
        [Required]
        [Display(Name = "Cantidad de Personas")]
        public int Cantidad_Personas { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public int Estado { get; set; }

        [Required]
        [Display(Name = "Experiencia")]
        public int ID_Concurrencia { get; set; }

        public List<ConcurrenciaDto> Concurrencias { get; set; } = new List<ConcurrenciaDto>();
    }
}