using CoffeeShop.Application.Events;
using CoffeeShop.Application.Orders.Services;
using CoffeeShop.Domain.Orders.Events;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Infrastructure.Orders.Handlers;

/// <summary>
/// Обробник події OrderStatusChangedEvent (ISP): залежить виключно від IOrderAnalyticsWriter.
/// </summary>
public class OrderStatusChangedNotificationHandler : IEventHandler<OrderStatusChangedEvent>
{
    private readonly IOrderAnalyticsWriter _analyticsWriter;
    private readonly ILogger<OrderStatusChangedNotificationHandler> _logger;

    public OrderStatusChangedNotificationHandler(
        IOrderAnalyticsWriter analyticsWriter,
        ILogger<OrderStatusChangedNotificationHandler> logger)
    {
        _analyticsWriter = analyticsWriter;
        _logger = logger;
    }

    public Task HandleAsync(OrderStatusChangedEvent evt, CancellationToken ct = default)
    {
        _analyticsWriter.RecordStatusChange();

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
