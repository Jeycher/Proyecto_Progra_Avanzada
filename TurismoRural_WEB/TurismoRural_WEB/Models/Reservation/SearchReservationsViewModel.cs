using System.Collections.Generic;

namespace TurismoRural_WEB.Models.Reservation
{
    public class SearchReservationsViewModel
    {
        public Reservation Reservation { get; set; }
        public List<EstadoDto> Estados { get; set; } = new List<EstadoDto>();
    }
}
