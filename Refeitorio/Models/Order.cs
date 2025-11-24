using Refeitorio.Models;

namespace Refeitorio.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
        public string UserEmail { get; set; } = ""; // from session
        public DateTime PurchasedAt { get; set; } = DateTime.Now;
    }
}