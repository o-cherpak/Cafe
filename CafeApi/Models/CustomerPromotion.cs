namespace CafeApi.Models;

public class CustomerPromotion
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; } = null!;

    public DateTime PurchasedAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public bool IsUsed { get; set; }
    public int? UsedInOrderId { get; set; }
    public Order? Order { get; set; }
}