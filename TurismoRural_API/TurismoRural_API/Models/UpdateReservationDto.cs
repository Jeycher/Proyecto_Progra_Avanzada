namespace TurismoRural_API.Models
{
    public class UpdateReservationDto
    {
        public int UserId { get; set; }
        public int FechaId { get; set; }
        public int CantidadPersonas { get; set; }
        public bool Estado { get; set; }
    }
}
