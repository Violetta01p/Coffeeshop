using CoffeeShop.Domain.Orders.Entities;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Infrastructure.Persistence;

/// <summary>
/// Репозиторій замовлень, що працює через InMemoryDataStore та InMemoryUnitOfWork (Scoped).
/// Операції запису реєструються як відкладені дії через Unit of Work.
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUnitOfWork _unitOfWork;

    public InMemoryOrderRepository(InMemoryDataStore dataStore, InMemoryUnitOfWork unitOfWork)
    {
        _dataStore = dataStore;
        _unitOfWork = unitOfWork;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _dataStore.Orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Order> orders = _dataStore.Orders.Values
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
        return Task.FromResult(orders);
    }

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        _unitOfWork.RegisterAction(() => _dataStore.Orders.TryAdd(order.Id, order));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _unitOfWork.RegisterAction(() => _dataStore.Orders[order.Id] = order);
        return Task.CompletedTask;
    }
}
