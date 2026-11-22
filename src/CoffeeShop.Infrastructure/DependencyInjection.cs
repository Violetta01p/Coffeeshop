using CoffeeShop.Application.Common.Time;
using CoffeeShop.Application.Events;
using CoffeeShop.Application.Orders.Services;
using CoffeeShop.Domain.Menu.Repositories;
using CoffeeShop.Domain.Orders.Contracts;
using CoffeeShop.Domain.Orders.Events;
using CoffeeShop.Domain.Orders.Repositories;
using CoffeeShop.Infrastructure.Events;
using CoffeeShop.Infrastructure.Gateways;
using CoffeeShop.Infrastructure.Menu;
using CoffeeShop.Infrastructure.Orders;
using CoffeeShop.Infrastructure.Orders.Analytics;
using CoffeeShop.Infrastructure.Orders.Handlers;
using CoffeeShop.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeShop.Infrastructure;

/// <summary>
/// Реєстрація залежностей інфраструктурного шару (Composition Root).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // ── Singleton: Провайдери часу та In-Memory сховища стану ──
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IMenuItemRepository, InMemoryMenuItemRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        // Сховище аналітики подій (єдиний екземпляр для Reader і Writer - ISP)
        services.AddSingleton<InMemoryOrderAnalyticsStore>();
        services.AddSingleton<IOrderAnalyticsReader>(sp => sp.GetRequiredService<InMemoryOrderAnalyticsStore>());
        services.AddSingleton<IOrderAnalyticsWriter>(sp => sp.GetRequiredService<InMemoryOrderAnalyticsStore>());
        services.AddSingleton<IOrderAnalyticsService>(sp => sp.GetRequiredService<InMemoryOrderAnalyticsStore>());

        // ── Scoped: Інтеграційні шлюзи та шина подій ──
        services.AddScoped<IMenuCatalogGateway, MenuCatalogGateway>();
        services.AddScoped<IEventBus, InProcessEventBus>();

        // ── Scoped: Обробники подій (Event Handlers) ──
        services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedNotificationHandler>();
        services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedAnalyticsHandler>();
        services.AddScoped<IEventHandler<OrderStatusChangedEvent>, OrderStatusChangedNotificationHandler>();

        return services;
    }
}
