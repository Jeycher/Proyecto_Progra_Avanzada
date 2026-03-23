namespace TurismoRural_API.Models
{
    public class CreateReservationDto
    {
        public int UserId { get; set; }
        public int FechaId { get; set; }
        public int CantidadPersonas { get; set; }
    }
}
