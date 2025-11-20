using Refeitorio.Models;
using System.Text.Json;

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

        public void SerializarDict()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "MenuData.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            var dataForJson = LunchByDate.ToDictionary(
                k => k.Key.ToString("yyyy-MM-dd"),
                v => new
                {
                    v.Value.Id,
                    v.Value.MainDish,
                    v.Value.Soup,
                    v.Value.Dessert,
                    Option = v.Value.Option.ToString()
                });

            string json = JsonSerializer.Serialize(dataForJson, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
    }
}
