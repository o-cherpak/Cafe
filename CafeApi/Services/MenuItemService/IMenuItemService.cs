using CafeApi.DTOs;
using CafeApi.Enums;

namespace CafeApi.Services.MenuItemService;

public interface IMenuItemService
{
    Task<IEnumerable<MenuItemDto>> GetAll(ItemCategory? category);
    Task<MenuItemDto> GetById(int id);
    Task<MenuItemDto> Create(CreateMenuItemDto dto);
    Task Update(int id, UpdateMenuItemDto dto);
    Task Delete(int id);
}