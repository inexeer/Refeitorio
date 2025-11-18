using Refeitorio.Models;

namespace Refeitorio.Services
{
    public class AdminMenuService
    {
        public List<MenuDay> Menus { get; set; } = new List<MenuDay>();
        public Dictionary<DateOnly, MenuDay> LunchByDate { get; private set; } = new();
        public List<MenuDay> GetAll() => Menus;

        public void Add(MenuDay menuDay)
        {
            menuDay.Id = Menus.Count > 0 ? Menus.Max(m => m.Id) + 1 : 1;
            Menus.Add(menuDay);
        }

        public void SaveLunch(MenuDay menuDay, DateOnly data)
        {
            LunchByDate[data] = menuDay;
        }
    }
}
