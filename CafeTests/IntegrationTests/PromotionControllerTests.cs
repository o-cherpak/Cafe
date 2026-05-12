using System.Net;
using System.Net.Http.Json;
using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTests.IntegrationTests;

public class PromotionControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PromotionControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAllTest()
    {
        var response = await _client.GetAsync("/api/promotion");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetActiveTest()
    {
        var dto = new CreatePromotionDto(
            "10%", null, 100, DiscountType.Percentage, 10
        );
        await _client.PostAsJsonAsync("/api/promotion", dto);

        var response = await _client.GetAsync("/api/promotion/active");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<PromotionDto>>();
        result.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
    }

    [Fact]
    public async Task GetByIdTest()
    {
        var dto = new CreatePromotionDto(
            "10%", null, 100, DiscountType.Percentage, 10
        );

        var createResponse = await _client.PostAsJsonAsync("/api/promotion", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<PromotionDto>();

        var response = await _client.GetAsync($"/api/promotion/{created!.Id}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PromotionDto>();
        result!.Name.Should().Be("10%");
        result.BonusCost.Should().Be(100);
    }

    [Fact]
    public async Task GetById_NotFoundTest()
    {
        var response = await _client.GetAsync("/api/promotion/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTest()
    {
        var dto = new CreatePromotionDto(
            "20 Off", "Discount", 200, DiscountType.FixedAmount, 20
        );

        var response = await _client.PostAsJsonAsync("/api/promotion", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<PromotionDto>();
        result!.Name.Should().Be("20 Off");
        result.BonusCost.Should().Be(200);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTest()
    {
        var createDto = new CreatePromotionDto(
            "10%", null, 100, DiscountType.Percentage, 10
        );
        
        var createResponse = await _client.PostAsJsonAsync("/api/promotion", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<PromotionDto>();

        var updateDto = new UpdatePromotionDto("Updated Name", null, 15, null);
        var putResponse = await _client.PutAsJsonAsync($"/api/promotion/{created!.Id}", updateDto);
        putResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/promotion/{created.Id}");
        var result = await getResponse.Content.ReadFromJsonAsync<PromotionDto>();

        result!.Name.Should().Be("Updated Name");
        result.DiscountValue.Should().Be(15);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Update_NotFoundTest()
    {
        var updateDto = new UpdatePromotionDto(null, null, null, false);
        var response = await _client.PutAsJsonAsync("/api/promotion/99999", updateDto);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}