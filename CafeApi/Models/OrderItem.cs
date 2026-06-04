using CafeApi.Interfaces;

namespace CafeApi.Models;

public class OrderItem : IEntity
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public int MenuItemId { get; private set; }
    public MenuItem MenuItem { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal Total => Quantity * UnitPrice;
    public decimal UnitPrice { get; private set; }

    private OrderItem()
    {
    }

    public static OrderItem Create(int menuItemId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");
        if (unitPrice < 0) throw new ArgumentException("Price cannot be negative");
        
        return new OrderItem
        {
            MenuItemId = menuItemId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public static OrderItem Create(MenuItem menuItem, int quantity)
    {
        if (!menuItem.IsAvailable)
            throw new InvalidOperationException($"{menuItem.Name} is unavailable");

        return Create(menuItem.Id, quantity, menuItem.Price);
    }
    
    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");
        Quantity = quantity;
    }
}