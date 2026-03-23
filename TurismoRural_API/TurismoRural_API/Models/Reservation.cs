namespace TurismoRural_API.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int FechaId { get; set; }
        public int UserId { get; set; }
        public int ExperienceId { get; set; }

        public string UserName { get; set; }
        public string ExperienceName { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}