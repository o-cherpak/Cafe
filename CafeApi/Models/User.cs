using CafeApi.Enums;
using CafeApi.Interfaces;

namespace CafeApi.Models;

public class User : IEntity
{
    public int Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public int? CustomerId { get; private set; }
    public Customer? Customer { get; set; }
    
    private User() { }

    public static User Create(string email, string password, UserRole role, int? customerId = null)
    {
        return new User
        {
            Email = email,
            PasswordHash = HashPassword(password),
            Role = role,
            CustomerId = customerId
        };
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}