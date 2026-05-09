using CafeApi.Enums;

namespace CafeApi.Models;

public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int BonusCost { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    
    public ICollection<CustomerPromotion> CustomerPromotions { get; set; } = [];
}