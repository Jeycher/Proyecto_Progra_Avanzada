namespace TurismoRural_API.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int HostId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}