namespace CafeApi.Models;

public class Customer {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int BonusPoints { get; set; }
    public DateTime RegisteredAt { get; set; }
    
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<CustomerPromotion> Promotions { get; set; } = [];
}