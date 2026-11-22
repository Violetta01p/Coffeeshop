using CoffeeShop.Domain.Common.Exceptions;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Enums;

namespace CoffeeShop.Domain.Orders.Entities;

/// <summary>
/// Агрегат замовлення (Aggregate Root контексту Orders).
/// </summary>
public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public CustomerName CustomerName { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public Money Discount { get; private set; } = Money.Zero;
    public string? AppliedDiscountName { get; private set; }

    // Для ORM
    private Order() { }

    public Order(Guid id, CustomerName customerName, IEnumerable<OrderItem> items)
    {
        var itemList = items.ToList();

        if (itemList.Count == 0)
            throw new DomainException("Замовлення не може бути порожнім — додайте хоча б одну позицію.");

        Id = id;
        CustomerName = customerName;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        _items.AddRange(itemList);
    }

    /// <summary>
    /// Вартість замовлення без урахування знижок.
    /// </summary>
    public Money Subtotal =>
        _items.Aggregate(Money.Zero, (sum, item) => sum.Add(item.Subtotal));

    /// <summary>
    /// Підсумкова сума замовлення до сплати (з урахуванням можливої знижки).
    /// </summary>
    public Money TotalAmount =>
        new(Math.Max(0m, Subtotal.Amount - Discount.Amount));

    /// <summary>
    /// Застосувати знижку до замовлення (відповідно до обраної IDiscountPolicy).
    /// </summary>
    public void ApplyDiscount(Money discount, string? discountName)
    {
        if (discount.Amount < 0)
            throw new DomainException("Знижка не може бути від'ємною.");

        if (discount.Amount > Subtotal.Amount)
            throw new DomainException("Знижка не може перевищувати загальну вартість замовлення.");

        Discount = discount;
        AppliedDiscountName = string.IsNullOrWhiteSpace(discountName) ? null : discountName.Trim();
    }

    /// <summary>
    /// Перевести замовлення у статус «Готується».
    /// </summary>
    public void StartPreparing()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(
                $"Почати приготування можна лише для замовлення зі статусом Pending. Поточний статус: {Status}.");

        Status = OrderStatus.Preparing;
    }

    /// <summary>
    /// Перевести замовлення у статус «Готове».
    /// </summary>
    public void MarkAsReady()
    {
        if (Status != OrderStatus.Preparing)
            throw new DomainException(
                $"Позначити як готове можна лише замовлення зі статусом Preparing. Поточний статус: {Status}.");

        Status = OrderStatus.Ready;
    }

    /// <summary>
    /// Перевести замовлення у статус «Видано».
    /// </summary>
    public void Complete()
    {
        if (Status != OrderStatus.Ready)
            throw new DomainException(
                $"Завершити можна лише замовлення зі статусом Ready. Поточний статус: {Status}.");

        Status = OrderStatus.Completed;
    }

    /// <summary>
    /// Скасувати замовлення. Не можна скасувати вже видане (Completed) або вже скасоване замовлення.
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new DomainException("Неможливо скасувати вже видане замовлення.");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Замовлення вже скасовано.");

        Status = OrderStatus.Cancelled;
    }
}
