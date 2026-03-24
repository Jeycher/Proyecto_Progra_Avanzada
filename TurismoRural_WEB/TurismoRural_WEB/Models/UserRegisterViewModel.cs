using System.ComponentModel.DataAnnotations;

namespace TurismoRural_WEB.Models
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string confirmarContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        public string telefono { get; set; } = string.Empty;

        public int id_Rol { get; set; } = 2;
    }
}
