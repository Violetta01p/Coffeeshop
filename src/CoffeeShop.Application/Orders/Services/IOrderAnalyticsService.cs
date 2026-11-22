namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Композитний інтерфейс аналітики замовлень, що об'єднує Reader та Writer.
/// </summary>
public interface IOrderAnalyticsService : IOrderAnalyticsReader, IOrderAnalyticsWriter
{
}
