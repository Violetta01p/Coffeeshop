using CoffeeShop.Application.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace CoffeeShop.Infrastructure.Events;
/// <summary>
/// Реалізація In-Process шини подій (Event Bus).
/// Знаходить усі зареєстровані реалізації IEventHandler для типу TEvent та асинхронно виконує їх.
/// </summary>
public class InProcessEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InProcessEventBus> _logger;

    public InProcessEventBus(IServiceProvider serviceProvider, ILogger<InProcessEventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public async Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default)
    {
        if (evt is null)
            return;
        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>().ToList();
        if (handlers.Count == 0)
        {
            _logger.LogDebug("[EventBus] Не знайдено обробників для події {EventType}", typeof(TEvent).Name);
            return;
        }
        _logger.LogInformation("[EventBus] Публікація події {EventType} (обробників: {Count})",
            typeof(TEvent).Name, handlers.Count);

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(evt, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EventBus] Помилка обробки події {EventType} обробником {HandlerType}",
                    typeof(TEvent).Name, handler.GetType().Name);
                throw;
            }
        }
    }
}
