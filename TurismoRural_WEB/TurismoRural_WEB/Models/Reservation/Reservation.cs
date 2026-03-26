namespace TurismoRural_WEB.Models.Reservation
{
    public class Reservation
    {
        public int ID_Reserva { get; set; } // ID_Reserva
        public DateTime Fecha_Reserva { get; set; }
        public int Cantidad_Personas { get; set; }
        public int Estado { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Concurrencia { get; set; } // la refencia de la experiencia   \
        public string NombreConcurrencia { get; set; }
        public string EstadoNombre { get; set; }
    }
}