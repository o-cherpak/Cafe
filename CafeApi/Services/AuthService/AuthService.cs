using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions;
using CafeApi.Models;
using CafeApi.Services.CustomerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CafeApi.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly CafeDbContext _db;
    private readonly ICustomerService _customerService;

    public AuthService(
        IConfiguration configuration,
        CafeDbContext db,
        ICustomerService customerService
    )
    {
        _configuration = configuration;
        _db = db;
        _customerService = customerService;
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new ("customerId", user.CustomerId?.ToString() ?? "")
        };

        var secret = _configuration["Jwt:Secret"]
                     ?? throw new InvalidOperationException("JWT Secret not found in configuration.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"] ?? "60")),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<AuthResponseDto> Register(RegisterDto dto)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existing is not null)
            throw new ConflictException("User with this email already exists");

        int? customerId = null;

        if (dto.Role == UserRole.Customer)
        {
            string name = dto.Name ?? dto.Email;
            
            var customerDto = await _customerService.Create(
                new CreateCustomerDto(name, dto.Email)
            );
            customerId = customerDto.Id;
        }

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CustomerId = customerId
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponseDto(CreateToken(user), user.Email, user.Role);
    }

    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        return new AuthResponseDto(CreateToken(user), user.Email, user.Role);
    }
}