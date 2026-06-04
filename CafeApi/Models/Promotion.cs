using CafeApi.Enums;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class Promotion : IEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int BonusCost { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    public ICollection<CustomerPromotion> CustomerPromotions { get; set; } = [];

    private Promotion() { }

    public static Promotion Create(
        string name,
        string? description,
        DiscountType type,
        decimal value,
        int bonusCost,
        bool isActive = true
    )
    {
        return new Promotion
        {
            Name = name,
            Description = description,
            DiscountType = type,
            DiscountValue = value,
            BonusCost = bonusCost,
            IsActive = isActive
        };
    }

    public void Update(string? name, string? description, decimal? discountValue, bool? isActive)
    {
        if (name != null) Name = name;
        if (description != null) Description = description;
        if (discountValue != null) DiscountValue = discountValue.Value;
        if (isActive != null) IsActive = isActive.Value;
    }

    public decimal CalculateDiscountedPrice(decimal originalPrice)
    {
        return DiscountType switch
        {
            DiscountType.Percentage => originalPrice * (1 - DiscountValue / 100),
            DiscountType.FixedAmount => Math.Max(0, originalPrice - DiscountValue),
            _ => originalPrice
        };
    }
}