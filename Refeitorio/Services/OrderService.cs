using Refeitorio.Models;
using System.Text.Json;

namespace Refeitorio.Services
{
    public class OrderService
    {
        private List<Order> Purchases { get; set; } = new();
        private readonly string _filePath;

        public OrderService()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Orders.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            LoadPurchases();
        }

        public void AddPurchase(Order order)
        {
            order.Id = Purchases.Count > 0 ? Purchases.Max(p => p.Id) + 1 : 1;
            Purchases.Add(order);
            SavePurchases();
        }

        public List<Order> GetAll() => Purchases;

        public List<Order> GetByUser(string email) =>
            Purchases.Where(p => p.UserEmail == email).ToList();

        private void SavePurchases()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Purchases, options);
            File.WriteAllText(_filePath, json);
        }

        private void LoadPurchases()
        {
            if (!File.Exists(_filePath))
            {
                Purchases = new List<Order>();
                return;
            }

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                Purchases = new List<Order>();
                return;
            }

            try
            {
                var loaded = JsonSerializer.Deserialize<List<Order>>(json);
                Purchases = loaded ?? new List<Order>();
            }
            catch
            {
                Purchases = new List<Order>();
            }
        }
    }
}