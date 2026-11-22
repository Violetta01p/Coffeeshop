using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Domain.Orders.Entities;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Прикладний сервіс запитів (Queries) — відповідає лише за читання замовлень (SRP / ISP).
/// </summary>
public class OrderQueryService : IOrderQueryService
{
    private readonly IOrderRepository _orderRepository;

    public OrderQueryService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        return order is null ? null : MapToResponse(order);
    }

    // ── Mapping ──

    public static OrderResponse MapToResponse(Order order) =>
        new(order.Id,
            order.CustomerName.Value,
            order.Status.ToString(),
            order.CreatedAt,
            order.TotalAmount.Amount,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.MenuItemId,
                i.ProductName,
                i.UnitPrice.Amount,
                i.Quantity,
                i.Subtotal.Amount)).ToList(),
            order.Discount.Amount,
            order.AppliedDiscountName);
}
