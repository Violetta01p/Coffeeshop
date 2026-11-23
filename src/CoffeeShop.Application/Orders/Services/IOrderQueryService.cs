using CoffeeShop.Application.Orders.Dtos;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Інтерфейс виключно для операцій читання замовлень (ISP / SRP).
/// Працює зі спеціалізованими DTO: OrderSummaryDto для списку та OrderDetailsDto для одного замовлення.
/// </summary>
public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderSummaryDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrderDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
