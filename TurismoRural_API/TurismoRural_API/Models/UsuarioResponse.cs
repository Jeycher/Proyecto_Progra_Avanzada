namespace TurismoRural_API.Models
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}