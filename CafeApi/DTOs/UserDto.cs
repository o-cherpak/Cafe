using CafeApi.Enums;

namespace CafeApi.DTOs;

public record RegisterDto(string Email, string Password, UserRole Role);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, string Email, UserRole Role);