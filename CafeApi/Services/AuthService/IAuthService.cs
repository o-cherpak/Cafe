using CafeApi.Models;

namespace CafeApi.Services;

public interface IAuthService
{
    string CreateToken(User user);
}