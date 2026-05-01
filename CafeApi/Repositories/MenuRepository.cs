using CafeApi.Data;
using CafeApi.Enums;
using CafeApi.Interfaces;
using CafeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Repositories;

public class MenuRepository : Repository<MenuItem>, IMenuItemRepository
{
    public MenuRepository(CafeDbContext db) : base(db)
    {
    }

    public async Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(ItemCategory category)
    {
        return await Db.MenuItems
            .Where(menuItem => menuItem.Category == category)
            .AsNoTracking()
            .ToListAsync();
    }
}