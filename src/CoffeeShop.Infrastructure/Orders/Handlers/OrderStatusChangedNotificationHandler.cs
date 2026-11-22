using CoffeeShop.Application.Events;
using CoffeeShop.Domain.Orders.Events;
using CoffeeShop.Infrastructure.Orders.Analytics;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Infrastructure.Orders.Handlers;

/// <summary>
/// Обробник події OrderStatusChangedEvent: імітує сповіщення гостя про зміну статусу його замовлення.
/// </summary>
public class OrderStatusChangedNotificationHandler : IEventHandler<OrderStatusChangedEvent>
{
    private readonly InMemoryOrderAnalyticsStore _analyticsStore;
    private readonly ILogger<OrderStatusChangedNotificationHandler> _logger;

    public OrderStatusChangedNotificationHandler(
        InMemoryOrderAnalyticsStore analyticsStore,
        ILogger<OrderStatusChangedNotificationHandler> logger)
    {
        _analyticsStore = analyticsStore;
        _logger = logger;
    }

    public Task HandleAsync(OrderStatusChangedEvent evt, CancellationToken ct = default)
    {
        _analyticsStore.RecordStatusChange();

        var message = evt.NewStatus switch
        {
            "Preparing" => $"«{evt.CustomerName}, бариста вже готує ваше замовлення!»",
            "Ready" => $"«{evt.CustomerName}, ваше замовлення #{evt.OrderId} ГОТОВЕ до видачі! Смачного!»",
            "Completed" => $"«{evt.CustomerName}, замовлення #{evt.OrderId} успішно видано. Дякуємо, що обрали нас!»",
            "Cancelled" => $"«{evt.CustomerName}, замовлення #{evt.OrderId} було скасовано.»",
            _ => $"«{evt.CustomerName}, статус вашого замовлення змінено на {evt.NewStatus}.»"
        };

        _logger.LogInformation(
            "📱 [Сповіщення гостя] Замовлення #{OrderId} (зміна: {OldStatus} → {NewStatus}): {NotificationMessage}",
            evt.OrderId,
            evt.PreviousStatus,
            evt.NewStatus,
            message);

        return Task.CompletedTask;
    }
}
