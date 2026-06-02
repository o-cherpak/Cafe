using CafeApi.Enums;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class User : IEntity
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}