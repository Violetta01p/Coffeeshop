using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Services;

namespace CoffeeShop.Infrastructure.Orders.Analytics;

/// <summary>
/// Потокобезпечне In-Memory сховище для накопичення статистики,
/// яку збирають Event-Driven обробники.
/// </summary>
public class InMemoryOrderAnalyticsStore : IOrderAnalyticsService
{
    private readonly object _lock = new();

    private int _totalOrdersCount;
    private decimal _totalRevenue;
    private DateTime? _lastOrderCreatedAtUtc;
    private int _statusChangesCount;

    public void RecordOrder(decimal amount, DateTime createdAtUtc)
    {
        lock (_lock)
        {
            _totalOrdersCount++;
            _totalRevenue += amount;
            _lastOrderCreatedAtUtc = createdAtUtc;
        }
    }

    public void RecordStatusChange()
    {
        lock (_lock)
        {
            _statusChangesCount++;
        }
    }

    public OrderAnalyticsDto GetStatistics()
    {
        lock (_lock)
        {
            return new OrderAnalyticsDto(
                _totalOrdersCount,
                _totalRevenue,
                _lastOrderCreatedAtUtc,
                _statusChangesCount);
        }
    }
}
