using CoffeeShop.Domain.Common.Exceptions;
using CoffeeShop.Domain.Common.ValueObjects;

namespace CoffeeShop.Domain.Orders.Entities;

/// <summary>
/// Сутність позиції замовлення.
/// Зберігає зафіксовану ціну та назву на момент оформлення замовлення.
/// </summary>
public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid MenuItemId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public Money UnitPrice { get; private set; } = default!;
    public int Quantity { get; private set; }

    // Для ORM
    private OrderItem() { }

    public OrderItem(Guid menuItemId, string productName, Money unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Назва продукту не може бути порожньою.");

        if (unitPrice.Amount <= 0)
            throw new DomainException("Ціна продукту повинна бути більшою за нуль.");

        if (quantity <= 0)
            throw new DomainException("Кількість повинна бути більшою за нуль.");

        Id = Guid.NewGuid();
        MenuItemId = menuItemId;
        ProductName = productName.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    /// <summary>
    /// Загальна вартість позиції = ціна × кількість.
    /// </summary>
    public Money Subtotal => UnitPrice.Multiply(Quantity);
}
