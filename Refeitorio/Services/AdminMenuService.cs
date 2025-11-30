using Refeitorio.Models;
using System.IO;
using System.Text.Json;

namespace Refeitorio.Services
{
    public class AdminMenuService
    {
        public List<MenuDay> Menus { get; set; } = new List<MenuDay>();

        public Dictionary<DateOnly, Dictionary<MenuOption, MenuDay>> LunchByDate { get; private set; } = new();

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
            if (!LunchByDate.ContainsKey(data))
                LunchByDate[data] = new Dictionary<MenuOption, MenuDay>();

            LunchByDate[data][menuDay.Option] = menuDay;
        }

        public void SerializarDict()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "MenuData.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            var dataForJson = LunchByDate.ToDictionary(
                k => k.Key.ToString("yyyy-MM-dd"),
                v => v.Value.ToDictionary(
                    opt => opt.Key.ToString(),
                    menu => new
                    {
                        menu.Value.Id,
                        menu.Value.MainDish,
                        menu.Value.Soup,
                        menu.Value.Dessert,
                        Option = menu.Value.Option.ToString()
                    }
                )
            );

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
                LunchByDate = new Dictionary<DateOnly, Dictionary<MenuOption, MenuDay>>();
                return;
            }

            try
            {
                var json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                {
                    LunchByDate = new Dictionary<DateOnly, Dictionary<MenuOption, MenuDay>>();
                    return;
                }


                var tempDict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(json);

                LunchByDate = new Dictionary<DateOnly, Dictionary<MenuOption, MenuDay>>();

                if (tempDict != null)
                {
                    foreach (var dayEntry in tempDict)
                    {
                        if (!DateOnly.TryParse(dayEntry.Key, out DateOnly date))
                            continue;

                        var optionMenus = new Dictionary<MenuOption, MenuDay>();
                        foreach (var optKvp in dayEntry.Value)
                        {
                            if (!Enum.TryParse<MenuOption>(optKvp.Key, true, out var option)) continue;

                            var obj = optKvp.Value;
                            int id = obj.GetProperty("Id").GetInt32();


                            var menuDay = Menus.FirstOrDefault(m => m.Id == id);

                            if (menuDay != null)
                            {
                                optionMenus[option] = menuDay;
                            }
                            else
                            {

                                optionMenus[option] = new MenuDay
                                {
                                    Id = id,
                                    MainDish = obj.TryGetProperty("MainDish", out var mainDishProp) ? mainDishProp.GetString() ?? "" : "",
                                    Soup = obj.TryGetProperty("Soup", out var soupProp) ? soupProp.GetString() ?? "" : "",
                                    Dessert = obj.TryGetProperty("Dessert", out var dessertProp) ? dessertProp.GetString() ?? "" : "",
                                    Option = option
                                };
                            }
                        }
                        if (optionMenus.Count > 0)
                            LunchByDate[date] = optionMenus;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar MenuData.json: {ex.Message}");
                LunchByDate = new Dictionary<DateOnly, Dictionary<MenuOption, MenuDay>>();
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

        public void RemoveLunch(DateOnly date, MenuOption option)
        {
            if (LunchByDate.TryGetValue(date, out var dayDict))
            {
                if (dayDict.ContainsKey(option))
                {
                    dayDict.Remove(option);
                    if (dayDict.Count == 0)
                        LunchByDate.Remove(date);
                }
            }
        }
    }
}