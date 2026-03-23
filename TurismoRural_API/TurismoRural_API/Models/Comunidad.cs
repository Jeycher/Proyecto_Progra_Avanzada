namespace TurismoRural_API.Models
{
    public class Comunidad
    {
        public int ID_Comunidad { get; set; }
        public string Nombre_Comunidad { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string? Provincia { get; set; }
        public string? Descripcion { get; set; }
    }
}
