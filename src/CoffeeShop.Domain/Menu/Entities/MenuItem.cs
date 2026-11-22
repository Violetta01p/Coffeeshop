using CoffeeShop.Domain.Common.Exceptions;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Menu.Enums;

namespace CoffeeShop.Domain.Menu.Entities;

/// <summary>
/// Сутність позиції меню (Aggregate Root контексту Menu).
/// </summary>
public class MenuItem
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public MenuCategory Category { get; private set; }
    public Money Price { get; private set; } = default!;
    public bool IsAvailable { get; private set; }

    // Для ORM / серіалізації
    private MenuItem() { }

    public MenuItem(Guid id, string name, MenuCategory category, Money price, bool isAvailable = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Назва позиції меню не може бути порожньою.");

        if (price.Amount <= 0)
            throw new DomainException("Ціна позиції меню повинна бути більшою за нуль.");

        Id = id;
        Name = name.Trim();
        Category = category;
        Price = price;
        IsAvailable = isAvailable;
    }

    /// <summary>
    /// Змінити ціну позиції.
    /// </summary>
    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Ціна позиції меню повинна бути більшою за нуль.");

        Price = newPrice;
    }

    /// <summary>
    /// Зробити позицію доступною.
    /// </summary>
    public void MarkAsAvailable() => IsAvailable = true;

    /// <summary>
    /// Зробити позицію недоступною.
    /// </summary>
    public void MarkAsUnavailable() => IsAvailable = false;
}
