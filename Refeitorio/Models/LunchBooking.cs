namespace Refeitorio.Models
{
    public class LunchBooking
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = "";
        public string Date { get; set; } = ""; // "2025-04-05"
        public string Option { get; set; } = ""; // "Normal" or "Vegetariano"
        public DateTime BookedAt { get; set; } = DateTime.Now;
    }
}
