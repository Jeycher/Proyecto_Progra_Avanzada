namespace TurismoRural_API.Models
{
    public class RegistroResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}
