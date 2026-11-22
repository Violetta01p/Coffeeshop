namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Інтерфейс виключно для запису аналітичних подій (ISP).
/// Використовується Event-обробниками.
/// </summary>
public interface IOrderAnalyticsWriter
{
    void RecordOrder(decimal amount, DateTime createdAtUtc);
    void RecordStatusChange();
}
