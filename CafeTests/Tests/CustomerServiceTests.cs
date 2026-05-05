using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Models;
using CafeApi.Repositories;
using CafeApi.Services.CustomerService;
using FluentAssertions;

namespace CafeTests.Tests;

public class CustomerServiceTests
{
    private readonly CafeDbContext _db;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _db = TestDbContextFactory.Create();

        var uow = new UnitOfWork(_db);
        _service = new CustomerService(uow);
    }

    private async Task SeedDb()
    {
        _db.AddRange(
            new Customer { Name = "Alex", Email = "emailC.com" },
            new Customer { Name = "Gabriel", Email = "emailB.com" },
            new Customer { Name = "Artur", Email = "emailA.com" }
        );

        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllTest()
    {
        await SeedDb();

        var result = await _service.GetAll();

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByIdTest()
    {
        await SeedDb();

        var result1 = await _service.GetById(1);
        var result2 = await _service.GetById(2);

        result1.Email.Should().Be("emailC.com");
        result2.Name.Should().Be("Gabriel");
    }

    [Fact]
    public async Task GetByEmailTest()
    {
        await SeedDb();

        var result1 = await _service.GetByEmail("emailA.com");
        var result2 = await _service.GetByEmail("emailC.com");

        result1.Name.Should().Be("Artur");
        result2.Name.Should().Be("Alex");
    }

    [Fact]
    public async Task CreateTest()
    {
        await _service.Create(
            new CreateCustomerDto("Alex", "alex@gmail.com")
        );

        _db.Customers.Should().HaveCount(1);

        var dto = await _service.Create(
            new CreateCustomerDto("Gabriel", "gabriel@gmail.com")
        );

        _db.Customers.Should().HaveCount(2);
        _db.Customers.Should().ContainSingle(c => c.Email == dto.Email && c.Name == dto.Name);
    }

    [Fact]
    public async Task UpdateTest()
    {
        await SeedDb();

        await _service.Update(
            id: 1,
            new UpdateCustomerDto("Czapka", null)
        );

        _db.Customers.Should().HaveCount(3);
        var updatedItem = await _service.GetById(1);

        updatedItem.Should().BeEquivalentTo(
            new CustomerDto(
                1, "Czapka", "emailC.com", 0
            )
        );
    }

    [Fact]
    public async Task DeleteTest()
    {
        await SeedDb();

        await _service.Delete(1);

        _db.Customers.Should().HaveCount(2);

        _db.Customers.Should().NotContain(c => c.Name == "Alex");
    }
}