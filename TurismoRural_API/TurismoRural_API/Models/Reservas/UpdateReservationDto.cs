namespace TurismoRural_API.Models.Reservas
{
    public class UpdateReservationDto
    {
        public int ID_Usuario { get; set; }
        public int ID_Fecha { get; set; }
        public int Cantidad_Personas { get; set; }
        public bool Estado { get; set; }
    }
}
