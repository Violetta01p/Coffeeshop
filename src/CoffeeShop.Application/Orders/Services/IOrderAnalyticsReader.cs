using CoffeeShop.Application.Orders.Dtos;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Інтерфейс виключно для читання аналітичної статистики (ISP).
/// Використовується контролером AnalyticsController.
/// </summary>
public interface IOrderAnalyticsReader
{
    OrderAnalyticsDto GetStatistics();
}
