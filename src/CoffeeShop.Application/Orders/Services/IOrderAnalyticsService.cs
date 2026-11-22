using CoffeeShop.Application.Orders.Dtos;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Інтерфейс сервісу аналітики замовлень.
/// </summary>
public interface IOrderAnalyticsService
{
    OrderAnalyticsDto GetStatistics();
}
