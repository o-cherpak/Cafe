using System.Net;
using System.Net.Http.Json;
using CafeApi.Data;
using CafeApi.DTOs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTests.IntegrationTests;

public class CustomerControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CustomerControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CafeDbContext>();
        db.Database.EnsureDeleted();
    }

    [Fact]
    public async Task GetAllTest()
    {
        var response = await _client.GetAsync("/api/customer");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetByEmailTest()
    {
        var dto = new CreateCustomerDto("Customer", "customer@gmail");
        var createResponse = await _client.PostAsJsonAsync("/api/customer", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var response = await _client.GetAsync($"/api/customer/by-email?email={created!.Email}");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CustomerDto>();
        result!.Name.Should().Be("Customer");
        result.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetByEmail_NotFoundTest()
    {
        var response = await _client.GetAsync("/api/customer/by-email?email=121212312");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        var dto = new CreateCustomerDto("Customer", "customer@gmail");
        var createResponse = await _client.PostAsJsonAsync("/api/customer", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var response = await _client.GetAsync($"/api/customer/{created!.Id}");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CustomerDto>();
        result!.Name.Should().Be("Customer");
        result.Email.Should().Be("customer@gmail");
    }

    [Fact]
    public async Task GetById_NotFoundTest()
    {
        var response = await _client.GetAsync("/api/customer/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTest()
    {
        var dto = new CreateCustomerDto("Customer", "customer@gmail");

        var response = await _client.PostAsJsonAsync("/api/customer", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CustomerDto>();
        result!.Name.Should().Be("Customer");
        result.Email.Should().Be("customer@gmail");
    }

    [Fact]
    public async Task UpdateTest()
    {
        var createDto = new CreateCustomerDto("Customer", "customer@gmail");
        var createResponse = await _client.PostAsJsonAsync("/api/customer", createDto);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        UpdateCustomerDto updateDto = new UpdateCustomerDto(null, "newEmail");
        var putResponse = await _client.PutAsJsonAsync($"/api/customer/{created!.Id}", updateDto);
        putResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/customer/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var result = await getResponse.Content.ReadFromJsonAsync<CustomerDto>();


        result!.Name.Should().Be("Customer");
        result.Email.Should().Be("newEmail");
    }

    [Fact]
    public async Task DeleteTest()
    {
        var createDto = new CreateCustomerDto("Customer", "customer@gmail");
        var createResponse = await _client.PostAsJsonAsync("/api/customer", createDto);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/customer/{created!.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/customer/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}