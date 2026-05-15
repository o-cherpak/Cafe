using CafeApi.DTOs;
using CafeApi.Models;

namespace CafeApi.Services;

public interface IAuthService
{
    string CreateToken(User user);
    Task<AuthResponseDto> Register(RegisterDto dto);
    Task<AuthResponseDto> Login(LoginDto dto);
}