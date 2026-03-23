namespace TurismoRural_API.Models
{
    public class Experiencia
    {
        public int ID_Experiencia { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public int? UsuarioIdRegistrador { get; set; }
        public int? ID_Comunidad { get; set; }
    }
}
