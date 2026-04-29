namespace CafeApi.Models;

public class Customer {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int BonusPoints { get; set; } = 0;
    public DateTime RegisteredAt { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
}