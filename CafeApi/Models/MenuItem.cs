using CafeApi.Enums;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class MenuItem : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ItemCategory Category { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Description { get; set; }
}