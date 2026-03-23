namespace TurismoRural_API.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ExperienceId { get; set; }
        public int UserId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}