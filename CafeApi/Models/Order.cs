using CafeApi.Enums;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class Order : IEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal FinalTotal { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
}