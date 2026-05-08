using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Exceptions;
using CafeApi.Exceptions.NotFoundExceptions;
using CafeApi.Interfaces;
using CafeApi.Models;

namespace CafeApi.Services.MenuItemService;

public class MenuItemService : IMenuItemService
{
    private IUnitOfWork _uow;

    public MenuItemService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private MenuItemDto ToDto(MenuItem menuItem)
    {
        var dto = new MenuItemDto(
            menuItem.Id,
            menuItem.Name,
            menuItem.Category,
            menuItem.Price,
            menuItem.IsAvailable
        );

        return dto;
    }

    public async Task<IEnumerable<MenuItemDto>> GetAll(ItemCategory? category)
    {
        IEnumerable<MenuItem> menuItems;

        if (category.HasValue)
        {
            menuItems = await _uow.MenuItems.GetItemsByCategoryAsync(category.Value);
        }
        else menuItems = await _uow.MenuItems.GetAllAsync();


        var result = menuItems.Select(ToDto);

        return result;
    }

    public async Task<MenuItemDto> GetById(int id)
    {
        var item = await _uow.MenuItems.GetByIdAsync(id);

        if (item is null)
            throw new MenuItemNotFound($"MenuItem with {id} id not found");

        return ToDto(item);
    }

    public async Task<MenuItemDto> Create(CreateMenuItemDto dto)
    {
        var item = new MenuItem
        {
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            Description = dto.Description
        };

        await _uow.MenuItems.AddAsync(item);

        await _uow.SaveChangesAsync();

        return ToDto(item);
    }

    public async Task Update(int id, UpdateMenuItemDto dto)
    {
        var item = await _uow.MenuItems.GetByIdAsync(id);

        if (item is null)
            throw new MenuItemNotFound($"MenuItem with {id} id not found");

        if (dto.Name is not null) item.Name = dto.Name;
        if (dto.Price is not null) item.Price = dto.Price.Value;
        if (dto.IsAvailable is not null) item.IsAvailable = dto.IsAvailable.Value;

        _uow.MenuItems.Update(item);
        await _uow.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var item = await _uow.MenuItems.GetByIdAsync(id);

        if (item is null) throw new MenuItemNotFound($"MenuItem with {id} id not found");
        

        _uow.MenuItems.Delete(item);
        await _uow.SaveChangesAsync();
    }
}