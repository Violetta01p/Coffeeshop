using CoffeeShop.Domain.Menu.Entities;

namespace CoffeeShop.Domain.Menu.Repositories;

/// <summary>
/// Репозиторій для роботи з позиціями меню.
/// </summary>
public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(MenuItem item, CancellationToken ct = default);
    Task UpdateAsync(MenuItem item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
