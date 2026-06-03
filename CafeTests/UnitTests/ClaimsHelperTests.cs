using System.Security.Claims;
using CafeApi.Helpers;
using FluentAssertions;

namespace CafeTests.UnitTests;

public class ClaimsHelperTests
{
    [Theory]
    [InlineData("Admin", 1, true)]
    [InlineData("Barista", 1, true)]
    [InlineData("Customer", 1, true)]
    [InlineData("Customer", 2, false)]
    public void HasAccessToCustomer_Test(string role, int customerId, bool expected
    )
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role)
        };

        if (role == "Customer")
        {
            claims.Add(new Claim("customerId", "1"));
        }

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var result = principal.HasAccessToCustomer(customerId);
        result.Should().Be(expected);
    }

    [Fact]
    public void HasAccessToCustomer_NoIdClaimTest()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Customer")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        
        var result = principal.HasAccessToCustomer(1);
        
        result.Should().BeFalse();
    }
}