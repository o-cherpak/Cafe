using System.Net;
using System.Net.Http.Json;
using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTests.IntegrationTests;

public class MenuItemControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MenuItemControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
        db.Database.EnsureDeleted();
    }

    [Fact]
    public async Task GetAllTest()
    {
        var response = await _client.GetAsync("/api/menuitem");
        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task GetByIdTest()
    {
        var dto = new CreateMenuItemDto("Latte", ItemCategory.Beverages, 75, null);
        var createResponse = await _client.PostAsJsonAsync("/api/menuitem", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<MenuItemDto>();

        var response = await _client.GetAsync($"/api/menuitem/{created!.Id}");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<MenuItemDto>();
        result!.Name.Should().Be("Latte");
        result.Price.Should().Be(75);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/menuitem/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTest()
    {
        var dto = new CreateMenuItemDto("Latte", ItemCategory.Beverages, 75, null);

        var response = await _client.PostAsJsonAsync("/api/menuitem", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<MenuItemDto>();
        result!.Name.Should().Be("Latte");
        result.Price.Should().Be(75);
    }

    [Fact]
    public async Task UpdateTest()
    {
        var createDto = new CreateMenuItemDto("Latte", ItemCategory.Beverages, 75, null);
        var createResponse = await _client.PostAsJsonAsync("/api/menuitem", createDto);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<MenuItemDto>();

        UpdateMenuItemDto updateDto = new UpdateMenuItemDto(null, 70, null);
        var putResponse = await _client.PutAsJsonAsync($"/api/menuitem/{created!.Id}", updateDto);
        putResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/menuitem/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var result = await getResponse.Content.ReadFromJsonAsync<MenuItemDto>();


        result!.Name.Should().Be("Latte");
        result.Price.Should().Be(70);
    }

    [Fact]
    public async Task DeleteTest()
    {
        var createDto = new CreateMenuItemDto("Latte", ItemCategory.Beverages, 75, null);
        var createResponse = await _client.PostAsJsonAsync("/api/menuitem", createDto);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<MenuItemDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/menuitem/{created!.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/menuitem/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}