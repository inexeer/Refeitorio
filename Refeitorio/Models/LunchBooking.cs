namespace Refeitorio.Models
{
    public class LunchBooking
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = "";
        public string Date { get; set; } = "";
        public string Option { get; set; } = "";
        public DateTime BookedAt { get; set; } = DateTime.Now;
    }
}
