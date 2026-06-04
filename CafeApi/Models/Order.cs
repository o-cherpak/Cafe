using CafeApi.Enums;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class Order : IEntity
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal FinalTotal { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = [];

    public decimal Total => Items.Sum(x => x.Total);

    private Order()
    {
    }

    public static Order Create(
        int customerId,
        ICollection<OrderItem> items
    )
    {
        var order = new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = items
        };
        order.FinalTotal = order.Total;
        return order;
    }

    public void ApplyPromotion(CustomerPromotion customerPromotion)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Promotion can only be applied to pending orders");

        FinalTotal = customerPromotion.Promotion.CalculateDiscountedPrice(Total);
        customerPromotion.MarkAsUsed(this);
    }

    public void UpdateStatus(OrderStatus status) => Status = status;
}