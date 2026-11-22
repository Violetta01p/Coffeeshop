namespace CoffeeShop.Domain.Orders.Enums;

/// <summary>
/// Статус приготування замовлення.
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Preparing = 1,
    Ready = 2,
    Completed = 3,
    Cancelled = 4
}
