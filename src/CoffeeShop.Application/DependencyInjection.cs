using CoffeeShop.Application.Common.Diagnostics;
using CoffeeShop.Application.Menu.Mappers;
using CoffeeShop.Application.Menu.Services;
using CoffeeShop.Application.Orders.Mappers;
using CoffeeShop.Application.Orders.Services;
using CoffeeShop.Domain.Orders.Pricing;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeShop.Application;

/// <summary>
/// Реєстрація залежностей шару Application (Composition Root).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ── Transient: Легковагові stateless-компоненти без стану ──
        services.AddTransient<IMenuMapper, MenuMapper>();
        services.AddTransient<IOrderMapper, OrderMapper>();
        services.AddTransient<ITransientService, TransientService>();

        // ── Scoped: Прикладні та доменні сервіси (один екземпляр на HTTP-запит) ──
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IOrderQueryService, OrderQueryService>();
        services.AddScoped<IOrderCommandService, OrderCommandService>();
        services.AddScoped<IOrderPricingService, OrderPricingService>();
        services.AddScoped<IScopedService, ScopedService>();

        // ── Singleton: Незмінні політики розрахунку знижок (stateless singletons) ──
        services.AddSingleton<IDiscountPolicy, BulkOrderDiscountPolicy>();
        services.AddSingleton<IDiscountPolicy, LargeOrderDiscountPolicy>();
        services.AddSingleton<ISingletonService, SingletonService>();

        return services;
    }
}
