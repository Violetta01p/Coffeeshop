using CoffeeShop.Application.Events;
using CoffeeShop.Application.Orders.Services;
using CoffeeShop.Domain.Orders.Events;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Infrastructure.Orders.Handlers;

/// <summary>
/// Обробник події OrderCreatedEvent (ISP): залежить виключно від IOrderAnalyticsWriter.
/// </summary>
public class OrderCreatedAnalyticsHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IOrderAnalyticsWriter _analyticsWriter;
    private readonly ILogger<OrderCreatedAnalyticsHandler> _logger;

    public OrderCreatedAnalyticsHandler(
        IOrderAnalyticsWriter analyticsWriter,
        ILogger<OrderCreatedAnalyticsHandler> logger)
    {
        _analyticsWriter = analyticsWriter;
        _logger = logger;
    }

    public Task HandleAsync(OrderCreatedEvent evt, CancellationToken ct = default)
    {
        _analyticsWriter.RecordOrder(evt.TotalAmount, evt.CreatedAtUtc);

        _logger.LogInformation(
            "📊 [Аналітика] Замовлення #{OrderId} враховано в аналітиці (сума: {TotalAmount:N2} грн).",
            evt.OrderId,
            evt.TotalAmount);

        return Task.CompletedTask;
    }
}
