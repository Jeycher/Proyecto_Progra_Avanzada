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
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public int Cupos_Disponibles { get; set; }
    }

    public class ExperienciaDto
    {
        public int ID_Experiencia { get; set; }
        public string Titulo { get; set; }
    }

    public class EstadoDto
    {
        public int iD_estado { get; set; }
        public int ID_Estado { get; set; }
        public string descripcion { get; set; }
        public string Nombre { get; set; }

        // Propiedad calculada para acceso flexible
        public int GetId() => iD_estado > 0 ? iD_estado : ID_Estado;
        public string GetNombre() => !string.IsNullOrEmpty(descripcion) ? descripcion : Nombre;
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
        public List<EstadoDto> Estados { get; set; } = new List<EstadoDto>();
    }
}