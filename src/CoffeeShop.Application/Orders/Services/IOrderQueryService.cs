using CoffeeShop.Application.Orders.Dtos;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Інтерфейс виключно для операцій читання замовлень (ISP / SRP).
/// Контролери та клієнти, яким потрібні лише дані замовлень, не залежать від команд мутації.
/// </summary>
public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken ct = default);
    Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
