using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Отримати всі замовлення.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAll(CancellationToken ct)
    {
        var orders = await _orderService.GetAllAsync(ct);
        return Ok(orders);
    }

    /// <summary>
    /// Отримати замовлення за Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken ct)
    {
        var order = await _orderService.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Створити нове замовлення.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(
        [FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var order = await _orderService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Змінити статус замовлення (prepare / ready / complete / cancel).
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> ChangeStatus(
        Guid id, [FromBody] ChangeStatusRequest request, CancellationToken ct)
    {
        var order = await _orderService.ChangeStatusAsync(id, request, ct);
        return Ok(order);
    }
}
