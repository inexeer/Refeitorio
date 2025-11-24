using Refeitorio.Models;
using System.IO;
using System.Text.Json;

namespace Refeitorio.Services
{
    public class AdminMenuService
    {
        public List<MenuDay> Menus { get; set; } = new List<MenuDay>();
        public Dictionary<DateOnly, MenuDay> LunchByDate { get; private set; } = new();
        public List<MenuDay> GetAll() => Menus;

        public AdminMenuService()
        {
            LoadMenus();
            LoadLunchByDate();
        }

        public void Add(MenuDay menuDay)
        {
            menuDay.Id = Menus.Count > 0 ? Menus.Max(m => m.Id) + 1 : 1;
            Menus.Add(menuDay);
            SaveMenus();
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

        private void LoadLunchByDate()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "MenuData.json");

            if (!File.Exists(path))
            {
                LunchByDate = new Dictionary<DateOnly, MenuDay>();
                return;
            }

            try
            {
                var json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                {
                    LunchByDate = new Dictionary<DateOnly, MenuDay>();
                    return;
                }

                var tempDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (tempDict == null || tempDict.Count == 0)
                {
                    LunchByDate = new Dictionary<DateOnly, MenuDay>();
                    return;
                }

                LunchByDate = new Dictionary<DateOnly, MenuDay>();

                foreach (var kvp in tempDict)
                {
                    if (!DateOnly.TryParse(kvp.Key, out DateOnly date))
                        continue;

                    var obj = kvp.Value;

                    if (!obj.TryGetProperty("Id", out JsonElement idElement))
                        continue;

                    int menuId = idElement.GetInt32();

                    var menuDay = Menus.FirstOrDefault(m => m.Id == menuId);

                    if (menuDay != null)
                    {
                        LunchByDate[date] = menuDay;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar MenuData.json: {ex.Message}");
                LunchByDate = new Dictionary<DateOnly, MenuDay>();
            }
        }

        public void SaveMenus()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Menus.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Menus, options);
            File.WriteAllText(path, json);
        }

        private void LoadMenus()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Menus.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            if (!File.Exists(path))
            {
                Menus = new List<MenuDay>();
                return;
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                Menus = new List<MenuDay>();
                return;
            }

            try
            {
                Menus = JsonSerializer.Deserialize<List<MenuDay>>(json) ?? new();
            }
            catch
            {
                Menus = new List<MenuDay>();
            }
        }
    }
}
