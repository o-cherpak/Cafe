using CafeApi.Enums;

namespace CafeApi.DTOs;

public record MenuItemDto(
    int Id,
    string Name,
    ItemCategory Category,
    decimal Price,
    bool IsAvailable
);

public record CreateMenuItemDto(
    string Name,
    ItemCategory Category,
    decimal Price,
    string? Description
);

public record UpdateMenuItemDto(string? Name, decimal? Price, bool? IsAvailable);