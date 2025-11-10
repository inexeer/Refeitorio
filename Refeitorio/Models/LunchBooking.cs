namespace Refeitorio.Models
{
    public class LunchBooking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int MenuDayId { get; set; }
        public MenuDay MenuDay { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Ativa";
    }
}
