using Refeitorio.Models;

namespace Refeitorio.Services
{
    public class AdminMenuService
    {
        public List<MenuDay> Menus { get; set; } = new List<MenuDay>();

        public void Add(MenuDay menuDay)
        {
            menuDay.Id = Menus.Count > 0 ? Menus.Max(m => m.Id) + 1 : 1;
            Menus.Add(menuDay);
        }
    }
}
