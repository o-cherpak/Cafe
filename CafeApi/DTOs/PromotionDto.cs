using CafeApi.Enums;

namespace CafeApi.DTOs;

public record PromotionDto(
    int Id,
    string Name,
    string? Description,
    int BonusCost,
    DiscountType DiscountType,
    decimal DiscountValue,
    bool IsActive
);

public record CreatePromotionDto(
    string Name,
    string? Description,
    int BonusCost,
    DiscountType DiscountType,
    decimal DiscountValue
);

public record UpdatePromotionDto(
    string? Name,
    string? Description,
    decimal? DiscountValue,
    bool? IsActive
);