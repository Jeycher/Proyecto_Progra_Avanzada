namespace TurismoRural_API.Models
{
    public class ExperienciaConcurrenciaUpdateRequest
    {
        public DateTime? Fecha { get; set; }
        public string? Detalle { get; set; }
        public decimal? Precio { get; set; }
        public int? Cupos_Disponibles { get; set; }
        public int? ID_Experiencia { get; set; }
    }
}
