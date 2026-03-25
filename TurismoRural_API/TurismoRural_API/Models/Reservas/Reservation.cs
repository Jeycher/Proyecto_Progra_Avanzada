namespace TurismoRural_API.Models.Reservas
{
    public class Reservation
    {
        public int ID_Reserva { get; set; } // ID_Reserva
        public DateTime Fecha_Reserva { get; set; }
        public int Cantidad_Personas { get; set; }
        public bool Estado { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Fecha { get; set; }


    }
}