using CafeApi.DTOs;
using CafeApi.Services.PromotionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PromotionDto>>> GetAll()
    {
        var result = await _promotionService.GetAll();

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<PromotionDto>>> GetActivePromotions()
    {
        var result = await _promotionService.GetActivePromotions();

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PromotionDto>> GetById(int id)
    {
        var dto = await _promotionService.GetById(id);

        return Ok(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PromotionDto>> Create(CreatePromotionDto createDto)
    {
        var result = await _promotionService.Create(createDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<PromotionDto>> Update(
        int id,
        [FromBody] UpdatePromotionDto updatePromotionDto
    )
    {
        await _promotionService.Update(id, updatePromotionDto);

        return NoContent();
    }
}