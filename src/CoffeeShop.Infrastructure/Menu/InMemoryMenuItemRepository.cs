using System.Collections.Concurrent;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Enums;
using CoffeeShop.Domain.Menu.Repositories;

namespace CoffeeShop.Infrastructure.Menu;

/// <summary>
/// In-Memory реалізація репозиторію позицій меню.
/// Містить початкові тестові дані (seed).
/// </summary>
public class InMemoryMenuItemRepository : IMenuItemRepository
{
    private readonly ConcurrentDictionary<Guid, MenuItem> _store = new();

    public InMemoryMenuItemRepository()
    {
        SeedData();
    }

    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<MenuItem> items = _store.Values.ToList();
        return Task.FromResult(items);
    }

    public Task AddAsync(MenuItem item, CancellationToken ct = default)
    {
        _store.TryAdd(item.Id, item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MenuItem item, CancellationToken ct = default)
    {
        _store[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    // ── Seed Data ──

    private void SeedData()
    {
        var items = new[]
        {
            new MenuItem(Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                "Еспресо", MenuCategory.HotDrink, new Money(55m)),

            new MenuItem(Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                "Капучино", MenuCategory.HotDrink, new Money(75m)),

            new MenuItem(Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                "Лате", MenuCategory.HotDrink, new Money(80m)),

            new MenuItem(Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                "Матча Лате", MenuCategory.HotDrink, new Money(95m)),

            new MenuItem(Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                "Круасан", MenuCategory.Dessert, new Money(65m)),
        };

        foreach (var item in items)
            _store.TryAdd(item.Id, item);
    }
}
