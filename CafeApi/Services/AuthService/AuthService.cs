using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions;
using CafeApi.Models;
using CafeApi.Services.CustomerService;
using CafeApi.Services.TokenService;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Services;

public class AuthService : IAuthService
{
    private readonly CafeDbContext _db;
    private readonly ICustomerService _customerService;
    private readonly ITokenService _tokenService;

    public AuthService(
        CafeDbContext db,
        ICustomerService customerService,
        ITokenService tokenService
    )
    {
        _db = db;
        _customerService = customerService;
        _tokenService = tokenService;
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

        var user = User.Create(dto.Email, dto.Password, dto.Role, customerId);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        return new AuthResponseDto(token, user.Email, user.Role, customerId);
    }

    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !user.VerifyPassword(dto.Password))
        {
            throw new UnauthorizedException("Invalid email or password");
        }
        
        var token = _tokenService.CreateToken(user);
        return new AuthResponseDto(token, user.Email, user.Role, user.CustomerId);
    }
}