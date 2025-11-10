using System;

namespace Refeitorio.Models
{
    public class Booking
    {
        public int DayId { get; set; }
        public int WeekNumber { get; set; }
        public MenuOption Option { get; set; }
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }
}