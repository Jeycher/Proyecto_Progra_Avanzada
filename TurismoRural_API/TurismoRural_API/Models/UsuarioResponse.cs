namespace TurismoRural_API.Models
{
    public class UsuarioResponse
    {
        public int Consecutivo { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasenna { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string ImagenPerfil { get; set; } = string.Empty;
    }
}