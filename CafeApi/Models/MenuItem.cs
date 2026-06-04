using CafeApi.Enums;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class MenuItem : IEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public ItemCategory Category { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public string? Description { get; private set; }

    private MenuItem()
    {
    }

    public static MenuItem Create(string name, ItemCategory category, decimal price, string? description)
    {
        return new MenuItem
        {
            Name = name,
            Category = category,
            Price = price,
            Description = description,
            IsAvailable = true
        };
    }

    public void Update(
        string? name,
        decimal? price,
        bool? isAvailable,
        ItemCategory? category,
        string? description
    )
    {
        if (name is not null) Name = name;
        if (price is not null) Price = price.Value;
        if (isAvailable is not null) IsAvailable = isAvailable.Value;
        if (category is not null) Category = category.Value;
        if (description is not null) Description = description;
    }
}