namespace CafeApi.DTOs;

public record CustomerDto(int Id, string Name, string Email, int BonusPoints);

public record CreateCustomerDto(string Name, string Email);

public record UpdateCustomerDto(string? Name, string? Email);