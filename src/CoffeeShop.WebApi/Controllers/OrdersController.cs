using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderCommandService _orderCommandService;
    private readonly IOrderQueryService _orderQueryService;

    public OrdersController(
        IOrderCommandService orderCommandService,
        IOrderQueryService orderQueryService)
    {
        _orderCommandService = orderCommandService;
        _orderQueryService = orderQueryService;
    }

    /// <summary>
    /// Отримати компактний список усіх замовлень (OrderSummaryDto без важкого масиву items).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryDto>>> GetAll(CancellationToken ct)
    {
        var orders = await _orderQueryService.GetAllAsync(ct);
        return Ok(orders);
    }

    /// <summary>
    /// Отримати повну інформацію про замовлення за Id (OrderDetailsDto з items та знижками).
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailsDto>> GetById(Guid id, CancellationToken ct)
    {
        var order = await _orderQueryService.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Створити нове замовлення (повертає деталізований OrderDetailsDto).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDetailsDto>> Create(
        [FromBody] CreateOrderRequestDto request, CancellationToken ct)
    {
        var order = await _orderCommandService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Змінити статус замовлення (повертає оновлений OrderDetailsDto).
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderDetailsDto>> ChangeStatus(
        Guid id, [FromBody] UpdateOrderStatusRequestDto request, CancellationToken ct)
    {
        var order = await _orderCommandService.ChangeStatusAsync(id, request, ct);
        return Ok(order);
    }
}
