using System.Net;
using System.Net.Http.Json;
using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeTests.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTests.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
        db.Database.EnsureDeleted();
    }

    [Fact]
    public async Task RegisterTest()
    {
        var dto = new RegisterDto(
            "Test User",
            "test@test.com",
            "Password123!",
            UserRole.Admin
        );

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result!.Email.Should().Be(dto.Email);
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginTest()
    {
        var registerDto = new RegisterDto(
            "Test User",
            "login@test.com",
            "Password123!",
            UserRole.Admin
        );

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto("login@test.com", "Password123!");

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result!.Email.Should().Be(loginDto.Email);
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_InvalidCredTest()
    {
        var loginDto = new LoginDto("nonexistent@test.com", "WrongPassword");

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}