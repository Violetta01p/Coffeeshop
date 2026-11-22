using System.Collections.Concurrent;
using CoffeeShop.Domain.Orders.Entities;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Infrastructure.Orders;

/// <summary>
/// In-Memory реалізація репозиторію замовлень.
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Order> orders = _store.Values
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
        return Task.FromResult(orders);
    }

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        _store.TryAdd(order.Id, order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _store[order.Id] = order;
        return Task.CompletedTask;
    }
}
