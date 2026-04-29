using CafeApi.Enums;
using CafeApi.Models;

namespace CafeApi.Interfaces;

public interface IMenuItemRepository : IRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(ItemCategory category);
}