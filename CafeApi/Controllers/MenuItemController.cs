using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Interfaces;
using CafeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public MenuItemController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAll([FromQuery] ItemCategory? category)
    {
        IEnumerable<MenuItem> menuItems;

        if (category.HasValue)
        {
            menuItems = await _uow.MenuItems.GetItemsByCategoryAsync(category.Value);
        }
        else menuItems = await _uow.MenuItems.GetAllAsync();


        var result = menuItems.Select(item =>
            new MenuItemDto(
                item.Id,
                item.Name,
                item.Category,
                item.Price,
                item.IsAvailable
            ));

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItemDto>> GetById(int id)
    {
        var item = await _uow.MenuItems.GetByIdAsync(id);

        if (item is null) return NotFound();

        MenuItemDto dto = new MenuItemDto(
            item.Id,
            item.Name,
            item.Category,
            item.Price,
            item.IsAvailable
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<MenuItemDto>> Create(CreateMenuItemDto dto)
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

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.Id },
            new MenuItemDto(
                item.Id,
                item.Name,
                item.Category,
                item.Price,
                item.IsAvailable
            )
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateMenuItemDto dto)
    {
        var item = await _uow.MenuItems.GetByIdAsync(id);

        if (item is null)
            return NotFound();

        if (dto.Name is not null) item.Name = dto.Name;
        if (dto.Price is not null) item.Price = dto.Price.Value;
        if (dto.IsAvailable is not null) item.IsAvailable = dto.IsAvailable.Value;

        _uow.MenuItems.Update(item);
        await _uow.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var item = await _uow.MenuItems.GetByIdAsync(id);

        if (item is null) return NotFound();

        _uow.MenuItems.Delete(item);
        await _uow.SaveChangesAsync();

        return NoContent();
    }
}