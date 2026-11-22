using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Domain.Orders.Repositories;

/// <summary>
/// Репозиторій для роботи із замовленнями.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
}
