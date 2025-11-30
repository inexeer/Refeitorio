using System;

namespace Refeitorio.Models
{
    public class MenuDay
    {
        public int Id { get; set; }
        public MenuOption Option { get; set; }
        public string MainDish { get; set; }
        public string Soup { get; set; }
        public string Dessert { get; set; }
    }
}