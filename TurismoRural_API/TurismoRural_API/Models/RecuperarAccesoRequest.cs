using System.ComponentModel.DataAnnotations;

namespace TurismoRural_API.Models
{
    public class RecuperarAccesoRequest
    {
        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; } = string.Empty;
    }
}
