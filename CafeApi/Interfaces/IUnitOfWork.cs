namespace CafeApi.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMenuItemRepository MenuItems { get; }
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    Task SaveChangesAsync();
}