using CafeApi.DTOs;
using CafeApi.Services.CustomerPromotionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomerPromotionController : ControllerBase
{
    private readonly ICustomerPromotionService _service;

    public CustomerPromotionController(ICustomerPromotionService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,Barista")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerPromotionDto>>> GetAll()
    {
        var result = await _service.GetAll();

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerPromotionDto>> GetById(int id)
    {
        var result = await _service.GetById(id);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpGet("by-customer")]
    public async Task<ActionResult<IEnumerable<CustomerPromotionDto>>>
        GetByCustomerIdAsync([FromQuery] int customerId)
    {
        var result =
            await _service.GetByCustomerIdAsync(customerId);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpGet("by-customer-promotion")]
    public async Task<ActionResult<CustomerPromotionDto>>
        GetByCustomerAndPromotionAsync(
            [FromQuery] int customerId, [FromQuery] int promotionId
        )
    {
        var result =
            await _service.GetByCustomerAndPromotionAsync(customerId, promotionId);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpGet("by-order")]
    public async Task<ActionResult<IEnumerable<CustomerPromotionDto>>>
        GetByOrderIdAsync([FromQuery] int orderId)
    {
        var result = await _service.GetByOrderAsync(orderId);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Barista,Customer")]
    [HttpPost]
    public async Task<ActionResult<CustomerPromotionDto>>
        BuyPromotion(BuyPromotionDto buyPromotionDto)
    {
        var dto = await _service.BuyPromotion(buyPromotionDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = dto.Id },
            dto
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.Delete(id);

        return NoContent();
    }
}