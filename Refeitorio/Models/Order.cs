using Refeitorio.Models;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }

    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }

    public ICollection<OrderItem> Items { get; set; }
}