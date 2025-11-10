using System.Collections.Generic;

namespace Refeitorio.Models
{
    public class MenuWeek
    {
        public int Id { get; set; } 
        public int WeekNumber { get; set; } 
        public List<MenuDay> MenuDays { get; set; } = new List<MenuDay>();
    }
}