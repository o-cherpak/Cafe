namespace CafeApi.DTOs;

public record CustomerPromotionDto(
    int Id,
    int CustomerId,
    PromotionDto Promotion,
    bool IsUsed,
    DateTime PurchasedAt,
    DateTime? UsedAt,
    int? UsedInOrderId
);

public record BuyPromotionDto(
    int CustomerId,
    int PromotionId
);