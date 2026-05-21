using System.Security.Claims;

namespace CafeApi.Helpers;

public static class ClaimsHelper
{
    private static bool IsAdminOrBarista(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin") || user.IsInRole("Barista");
    }

    public static bool HasAccessToCustomer(this ClaimsPrincipal user, int requestedCustomerId)
    {
        if (user.IsAdminOrBarista()) return true;
        
        var jwtCustomerId = user.FindFirst("customerId")?.Value;
        
        return jwtCustomerId == requestedCustomerId.ToString();
    }
}