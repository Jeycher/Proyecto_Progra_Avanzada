namespace TurismoRural_API.Models
{
    public class ExperienciaUpdateRequest
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public int? UsuarioIdRegistrador { get; set; }
        public int? ID_Comunidad { get; set; }
    }
}
