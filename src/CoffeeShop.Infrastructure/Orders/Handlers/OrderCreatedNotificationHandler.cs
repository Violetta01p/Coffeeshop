using CoffeeShop.Application.Events;
using CoffeeShop.Domain.Orders.Events;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Infrastructure.Orders.Handlers;

/// <summary>
/// Обробник події OrderCreatedEvent: імітує сповіщення персоналу кухні / баристи.
/// </summary>
public class OrderCreatedNotificationHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedNotificationHandler> _logger;

    public OrderCreatedNotificationHandler(ILogger<OrderCreatedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderCreatedEvent evt, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "🔔 [Бариста / Кухня] Нове замовлення #{OrderId} від гостя '{CustomerName}'! Позицій: {Count}, до сплати: {Amount:N2} грн. Розпочніть підготовку.",
            evt.OrderId,
            evt.CustomerName,
            evt.ItemsCount,
            evt.TotalAmount);

        return Task.CompletedTask;
    }
}
