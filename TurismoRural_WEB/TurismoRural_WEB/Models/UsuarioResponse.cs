namespace TurismoRural_WEB.Models
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int Rol { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
