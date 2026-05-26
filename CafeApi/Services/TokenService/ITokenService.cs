using CafeApi.Models;

namespace CafeApi.Services.TokenService;

public interface ITokenService
{
    string CreateToken(User user);
}