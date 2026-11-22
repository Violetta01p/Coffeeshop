using CoffeeShop.Application.Menu.Services;
using CoffeeShop.Application.Orders.Services;
using CoffeeShop.Domain.Menu.Repositories;
using CoffeeShop.Domain.Orders.Contracts;
using CoffeeShop.Domain.Orders.Repositories;
using CoffeeShop.Infrastructure.Gateways;
using CoffeeShop.Infrastructure.Menu;
using CoffeeShop.Infrastructure.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeShop.Infrastructure;

/// <summary>
/// Метод розширення для реєстрації залежностей інфраструктурного шару.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Репозиторії (Singleton, бо InMemory-сховище має жити весь час)
        services.AddSingleton<IMenuItemRepository, InMemoryMenuItemRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        // Контракт взаємодії між контекстами
        services.AddScoped<IMenuCatalogGateway, MenuCatalogGateway>();

        // Прикладні сервіси
        services.AddScoped<MenuService>();
        services.AddScoped<OrderService>();

        return services;
    }
}
