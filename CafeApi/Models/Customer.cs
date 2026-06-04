using CafeApi.Exceptions;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class Customer : IEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public int BonusPoints { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<CustomerPromotion> Promotions { get; set; } = [];

    private Customer()
    {
    }

    public static Customer Create(string name, string email, int bonusPoint = 0)
    {
        return new Customer
        {
            Name = name,
            Email = email,
            BonusPoints = bonusPoint,
            RegisteredAt = DateTime.UtcNow
        };
    }

    public void Update(string? name, string? email)
    {
        if (name is not null) Name = name;
        if (email is not null) Email = email;
    }

    public void AddBonusPoints(int points)
    {
        if (points < 0) throw new ArgumentException("Points to add must be positive");
        BonusPoints += points;
    }

    public void SubtractBonusPoints(int points)
    {
        if (points < 0) throw new ArgumentException("Points to subtract must be positive");
        if (BonusPoints < points) throw new InsufficientBonusException("Not enough bonus points");
        BonusPoints -= points;
    }
}