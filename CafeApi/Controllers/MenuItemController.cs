using CafeApi.DTOs;
using CafeApi.Enums;
using CafeApi.Interfaces;
using CafeApi.Services.MenuItemService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenuItemController : ControllerBase
{
    private readonly IMenuItemService _menuItemService;

    public MenuItemController
    (
        IMenuItemService menuItemService
    )
    {
        _menuItemService = menuItemService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAll
    (
        [FromQuery] ItemCategory? category
    )
    {
        var result = await _menuItemService.GetAll(category);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItemDto>> GetById(int id)
    {
        var dto = await _menuItemService.GetById(id);

        return Ok(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MenuItemDto>> Create(CreateMenuItemDto dto)
    {
        var responseDto = await _menuItemService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = responseDto.Id },
            responseDto
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateMenuItemDto dto)
    {
        await _menuItemService.Update(id, dto);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _menuItemService.Delete(id);

        return NoContent();
    }
}