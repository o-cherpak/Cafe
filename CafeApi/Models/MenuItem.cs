using CafeApi.Enums;

namespace CafeApi.Models;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ItemCategory Category { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Description { get; set; }
}