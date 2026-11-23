using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Application.Menu.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Отримати всі позиції меню (список MenuItemSummaryDto).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MenuItemSummaryDto>>> GetAll(CancellationToken ct)
    {
        var items = await _menuService.GetAllAsync(ct);
        return Ok(items);
    }

    /// <summary>
    /// Отримати позицію меню за Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MenuItemSummaryDto>> GetById(Guid id, CancellationToken ct)
    {
        var item = await _menuService.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Додати нову позицію до меню (категорія передається рядком, наприклад "HotDrink").
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MenuItemSummaryDto>> Create(
        [FromBody] CreateMenuItemRequestDto request, CancellationToken ct)
    {
        var item = await _menuService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Змінити доступність позиції меню.
    /// </summary>
    [HttpPatch("{id:guid}/availability")]
    public async Task<ActionResult<MenuItemSummaryDto>> UpdateAvailability(
        Guid id, [FromBody] UpdateMenuItemAvailabilityRequestDto request, CancellationToken ct)
    {
        var item = await _menuService.UpdateAvailabilityAsync(id, request, ct);
        return Ok(item);
    }

    /// <summary>
    /// Змінити ціну позиції меню.
    /// </summary>
    [HttpPatch("{id:guid}/price")]
    public async Task<ActionResult<MenuItemSummaryDto>> UpdatePrice(
        Guid id, [FromBody] UpdateMenuItemPriceRequestDto request, CancellationToken ct)
    {
        var item = await _menuService.UpdatePriceAsync(id, request, ct);
        return Ok(item);
    }
}
