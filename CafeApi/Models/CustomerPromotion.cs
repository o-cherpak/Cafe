using CafeApi.Interfaces;

namespace CafeApi.Models;

public class CustomerPromotion : IEntity
{
    public int Id { get; private set; }

    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public int PromotionId { get; private set; }
    public Promotion Promotion { get; private set; } = null!;

    public DateTime PurchasedAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    public bool IsUsed { get; private set; }
    public int? UsedInOrderId { get; private set; }
    public Order? Order { get; private set; }

    private CustomerPromotion()
    {
    }

    public static CustomerPromotion Create(int customerId, int promotionId)
    {
        return new CustomerPromotion
        {
            CustomerId = customerId,
            PromotionId = promotionId,
            PurchasedAt = DateTime.UtcNow
        };
    }

    public void MarkAsUsed(Order order)
    {
        if (IsUsed) 
            throw new InvalidOperationException("Promotion already used");
        
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        Order = order;
        UsedInOrderId = order.Id;
    }
}