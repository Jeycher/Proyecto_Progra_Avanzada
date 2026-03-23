using System.ComponentModel.DataAnnotations;

namespace TurismoRural_API.Models
{
    public class ExperienciaCreateRequest
    {
        [Required]
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        [Required]
        public int UsuarioIdRegistrador { get; set; }
        [Required]
        public int ID_Comunidad { get; set; }
    }
}
