using CoffeeShop.Application.Orders.Dtos;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Інтерфейс виключно для команд створення та зміни стану замовлень (ISP / SRP).
/// </summary>
public interface IOrderCommandService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderResponse> ChangeStatusAsync(Guid id, ChangeStatusRequest request, CancellationToken ct = default);
}
