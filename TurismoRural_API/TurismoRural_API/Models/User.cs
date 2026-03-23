namespace TurismoRural_API.Models
{
    
    public class User
    {
        public int ID_Usuario { get; set; }
        public string? Nombre { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string? Contrasena { get; set; }
        public int Telefono { get; set; }
        public string ID_Rol { get; set; } = "User";
        public DateTime Fecha_Registro { get; set; } = DateTime.UtcNow;
    }
}