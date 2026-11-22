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
    /// Отримати всі замовлення (через IOrderQueryService - ISP/SRP/DIP).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAll(CancellationToken ct)
    {
        var orders = await _orderQueryService.GetAllAsync(ct);
        return Ok(orders);
    }

    /// <summary>
    /// Отримати замовлення за Id (через IOrderQueryService - ISP/SRP/DIP).
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken ct)
    {
        var order = await _orderQueryService.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Створити нове замовлення (через IOrderCommandService - ISP/SRP/DIP).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(
        [FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var order = await _orderCommandService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Змінити статус замовлення (через IOrderCommandService - ISP/SRP/DIP).
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> ChangeStatus(
        Guid id, [FromBody] ChangeStatusRequest request, CancellationToken ct)
    {
        var order = await _orderCommandService.ChangeStatusAsync(id, request, ct);
        return Ok(order);
    }
}
