using CoffeeShop.Application.Events;
using CoffeeShop.Application.Menu.Services;
using CoffeeShop.Application.Orders.Services;
using CoffeeShop.Domain.Menu.Repositories;
using CoffeeShop.Domain.Orders.Contracts;
using CoffeeShop.Domain.Orders.Events;
using CoffeeShop.Domain.Orders.Pricing;
using CoffeeShop.Domain.Orders.Repositories;
using CoffeeShop.Infrastructure.Events;
using CoffeeShop.Infrastructure.Gateways;
using CoffeeShop.Infrastructure.Menu;
using CoffeeShop.Infrastructure.Orders;
using CoffeeShop.Infrastructure.Orders.Analytics;
using CoffeeShop.Infrastructure.Orders.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeShop.Infrastructure;

/// <summary>
/// Метод розширення для реєстрації залежностей інфраструктурного шару (DIP, ISP, OCP).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // ── Репозиторії (Singleton для In-Memory сховищ) ──
        services.AddSingleton<IMenuItemRepository, InMemoryMenuItemRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        // ── Контракт взаємодії між контекстами ──
        services.AddScoped<IMenuCatalogGateway, MenuCatalogGateway>();

        // ── Сховище аналітики подій (ISP: розділені Reader і Writer) ──
        services.AddSingleton<InMemoryOrderAnalyticsStore>();
        services.AddSingleton<IOrderAnalyticsReader>(sp => sp.GetRequiredService<InMemoryOrderAnalyticsStore>());
        services.AddSingleton<IOrderAnalyticsWriter>(sp => sp.GetRequiredService<InMemoryOrderAnalyticsStore>());
        services.AddSingleton<IOrderAnalyticsService>(sp => sp.GetRequiredService<InMemoryOrderAnalyticsStore>());

        // ── Політики знижок та ціноутворення (OCP) ──
        services.AddSingleton<IDiscountPolicy, BulkOrderDiscountPolicy>();
        services.AddSingleton<IDiscountPolicy, LargeOrderDiscountPolicy>();
        services.AddScoped<IOrderPricingService, OrderPricingService>();

        // ── Event-Driven інфраструктура (Event Bus) ──
        services.AddScoped<IEventBus, InProcessEventBus>();

        // ── Обробники подій (Event Handlers) ──
        services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedNotificationHandler>();
        services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedAnalyticsHandler>();
        services.AddScoped<IEventHandler<OrderStatusChangedEvent>, OrderStatusChangedNotificationHandler>();

        // ── Прикладні сервіси (DIP / SRP / ISP: інтерфейси замість конкретних класів) ──
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IOrderQueryService, OrderQueryService>();
        services.AddScoped<IOrderCommandService, OrderCommandService>();

        return services;
    }
}
