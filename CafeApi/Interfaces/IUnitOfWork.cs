namespace CafeApi.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMenuItemRepository MenuItems { get; }
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }
    IPromotionRepository Promotions { get; }
    ICustomerPromotionRepository CustomerPromotions { get; }
    Task SaveChangesAsync();
}