using System.ComponentModel.DataAnnotations;

namespace TurismoRural_WEB.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;
    }
}

