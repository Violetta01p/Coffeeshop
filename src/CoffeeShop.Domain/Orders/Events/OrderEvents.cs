namespace CoffeeShop.Domain.Orders.Events;

/// <summary>
/// Доменна подія, що сигналізує про успішне створення нового замовлення.
/// Незмінний record з мінімально необхідним набором даних.
/// </summary>
public sealed record OrderCreatedEvent(
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount,
    int ItemsCount,
    DateTime CreatedAtUtc);

/// <summary>
/// Доменна подія, що сигналізує про зміну життєвого циклу (статусу) замовлення.
/// </summary>
public sealed record OrderStatusChangedEvent(
    Guid OrderId,
    string CustomerName,
    string PreviousStatus,
    string NewStatus,
    DateTime ChangedAtUtc);
