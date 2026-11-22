using System.Collections.Concurrent;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Enums;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Infrastructure.Persistence;

/// <summary>
/// Єдине централізоване In-Memory сховище стану застосунку (Singleton).
/// Зберігає спільні потокобезпечні колекції меню та замовлень для всього додатку.
/// </summary>
public class InMemoryDataStore
{
    public ConcurrentDictionary<Guid, MenuItem> MenuItems { get; } = new();
    public ConcurrentDictionary<Guid, Order> Orders { get; } = new();

    public InMemoryDataStore()
    {
        SeedMenu();
    }

    private void SeedMenu()
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
        {
            MenuItems.TryAdd(item.Id, item);
        }
    }
}
