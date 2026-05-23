using CafeApi.Data;
using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions;
using CafeApi.Models;
using CafeApi.Services;
using CafeApi.Services.CustomerService;
using CafeApi.Validators.UserValidators;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CafeTests.UnitTests;

public class AuthServiceTests : IDisposable
{
    private readonly CafeDbContext _db;
    private readonly Mock<ICustomerService> _customerService;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _db = TestDbContextFactory.Create();
        var jwtSettings = new Dictionary<string, string>
        {
            { "Jwt:Secret", "cky0utCJCgY0SBzBAlLoQ2trxlWtT7cMHtdXcla1Un8" },
            { "Jwt:DurationInMinutes", "60" },
            { "Jwt:Issuer", "CafeApi" },
            { "Jwt:Audience", "CafeCustomers" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(jwtSettings!)
            .Build();

        _customerService = new Mock<ICustomerService>();
        _service = new AuthService(configuration, _db, _customerService.Object);
    }

    private async Task SeedDb()
    {
        _db.Users.Add(new User
        {
            Email = "alex@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123123"),
            Role = UserRole.Customer,
            CustomerId = 1
        });
        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterAdmin_WithoutCustomerIdTest()
    {
        var dto = new RegisterDto(
            "admin", "admin@gmail.com", "SecurePass123", UserRole.Admin
        );

        var result = await _service.Register(dto);

        result.Should().NotBeNull();
        result.Email.Should().Be(dto.Email);
        result.Role.Should().Be(UserRole.Admin);
        result.CustomerId.Should().BeNull();
        result.Token.Should().NotBeNullOrEmpty();

        var userInDb = _db.Users.Single(u => u.Email == dto.Email);
        BCrypt.Net.BCrypt.Verify(dto.Password, userInDb.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task Register_CustomerTest()
    {
        var dto = new RegisterDto(
            "user", "user@gmail.com", "123123", UserRole.Customer
        );

        var mockedCustomerId = 100;

        _customerService
            .Setup(c => c.Create(It.IsAny<CreateCustomerDto>()))
            .ReturnsAsync(
                new CustomerDto(mockedCustomerId, dto.Name!, dto.Email, 0)
            );

        var result = await _service.Register(dto);

        var userInDb = _db.Users.Single(u => u.Email == dto.Email);
        result.CustomerId.Should().NotBeNull();
        userInDb.CustomerId.Should().BeOfType(typeof(int));
    }

    [Fact]
    public async Task Register_ConflictExTest()
    {
        await SeedDb();
        var dto = new RegisterDto(
            "user", "alex@gmail.com", "123123", UserRole.Customer
        );

        await Assert.ThrowsAsync<ConflictException>(() => _service.Register(dto));
    }
    
    [Fact]
    public async Task Validator_RegisterNoErrorsTest()
    {
        var dto = new RegisterDto(
            "John", 
            "test@cafe.com", 
            "secure123", 
            UserRole.Customer
            );
        var validator = new RegisterValidator();
        
        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_RegisterInvalidTest()
    {
        var dto = new RegisterDto("Alex", "email", "0", (UserRole)99);
        var validator = new RegisterValidator();
        
        var result = await validator.ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    [Fact]
    public async Task LoginTest()
    {
        await SeedDb();
        var dto = new LoginDto("alex@gmail.com", "123123");

        var result = await _service.Login(dto);

        result.Should().NotBeNull();
        result.Email.Should().Be(dto.Email);
        result.CustomerId.Should().Be(1);
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("alex@gmail.com", "111111")]
    [InlineData("notFound@gmial.com", "1111")]
    public async Task Login_UnAuthTest(string email, string password)
    {
        await SeedDb();
        var dto = new LoginDto(email, password);

        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.Login(dto));
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }
}