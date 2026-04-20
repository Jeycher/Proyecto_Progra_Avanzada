using System.Collections.Generic;

namespace TurismoRural_WEB.Models.Reservation
{
    public class MyReservationsViewModel
    {
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public List<EstadoDto> Estados { get; set; } = new List<EstadoDto>();
    }
}
