namespace TurismoRural_WEB.Models
{
    public class UserViewModel
    {
        public int id_Usuario { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public string contrasena { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public int id_Rol { get; set; }
        public DateTime fecha_Registro { get; set; }
    }
}

