using CoffeeShop.Application.Events;
using CoffeeShop.Domain.Orders.Events;
using CoffeeShop.Infrastructure.Orders.Analytics;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Infrastructure.Orders.Handlers;

/// <summary>
/// Обробник події OrderCreatedEvent: накопичує бізнес-аналітику та виручку в пам'яті.
/// </summary>
public class OrderCreatedAnalyticsHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly InMemoryOrderAnalyticsStore _analyticsStore;
    private readonly ILogger<OrderCreatedAnalyticsHandler> _logger;

    public OrderCreatedAnalyticsHandler(
        InMemoryOrderAnalyticsStore analyticsStore,
        ILogger<OrderCreatedAnalyticsHandler> logger)
    {
        _analyticsStore = analyticsStore;
        _logger = logger;
    }

    public Task HandleAsync(OrderCreatedEvent evt, CancellationToken ct = default)
    {
        _analyticsStore.RecordOrder(evt.TotalAmount, evt.CreatedAtUtc);

        var stats = _analyticsStore.GetStatistics();

        _logger.LogInformation(
            "📊 [Аналітика] Замовлення #{OrderId} враховано в аналітиці. Всього замовлень: {TotalCount}, загальний виторг: {TotalRevenue:N2} грн.",
            evt.OrderId,
            stats.TotalOrdersCount,
            stats.TotalRevenue);

        return Task.CompletedTask;
    }
}
