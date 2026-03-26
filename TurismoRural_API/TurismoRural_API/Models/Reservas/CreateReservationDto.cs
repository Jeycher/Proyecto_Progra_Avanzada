namespace TurismoRural_API.Models.Reservas
{
    public class CreateReservationDto
    {
        public int ID_Usuario { get; set; }
        public int ID_Concurrencia { get; set; }
        public int Cantidad_Personas { get; set; }
        public int Estado { get; set; }
    }
}