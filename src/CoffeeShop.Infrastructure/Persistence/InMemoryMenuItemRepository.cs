using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Repositories;

namespace CoffeeShop.Infrastructure.Persistence;

/// <summary>
/// Репозиторій меню, що працює через InMemoryDataStore та InMemoryUnitOfWork (Scoped).
/// Операції запису реєструються як відкладені дії через Unit of Work.
/// </summary>
public class InMemoryMenuItemRepository : IMenuItemRepository
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUnitOfWork _unitOfWork;

    public InMemoryMenuItemRepository(InMemoryDataStore dataStore, InMemoryUnitOfWork unitOfWork)
    {
        _dataStore = dataStore;
        _unitOfWork = unitOfWork;
    }

    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _dataStore.MenuItems.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<MenuItem> items = _dataStore.MenuItems.Values.ToList();
        return Task.FromResult(items);
    }

    public Task AddAsync(MenuItem item, CancellationToken ct = default)
    {
        _unitOfWork.RegisterAction(() => _dataStore.MenuItems.TryAdd(item.Id, item));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MenuItem item, CancellationToken ct = default)
    {
        _unitOfWork.RegisterAction(() => _dataStore.MenuItems[item.Id] = item);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _unitOfWork.RegisterAction(() => _dataStore.MenuItems.TryRemove(id, out _));
        return Task.CompletedTask;
    }
}
