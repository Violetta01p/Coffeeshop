using CoffeeShop.Application.Events;
using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Domain.Common.Exceptions;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Contracts;
using CoffeeShop.Domain.Orders.Entities;
using CoffeeShop.Domain.Orders.Events;
using CoffeeShop.Domain.Orders.Pricing;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Прикладний сервіс команд (Commands) — відповідає за зміну стану замовлень,
/// розрахунок ціни через IOrderPricingService (OCP) та публікацію подій (SRP / ISP).
/// </summary>
public class OrderCommandService : IOrderCommandService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuCatalogGateway _menuCatalog;
    private readonly IEventBus _eventBus;
    private readonly IOrderPricingService _pricingService;

    public OrderCommandService(
        IOrderRepository orderRepository,
        IMenuCatalogGateway menuCatalog,
        IEventBus eventBus,
        IOrderPricingService pricingService)
    {
        _orderRepository = orderRepository;
        _menuCatalog = menuCatalog;
        _eventBus = eventBus;
        _pricingService = pricingService;
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new DomainException("Замовлення повинно містити хоча б одну позицію.");

        var orderItems = new List<OrderItem>();

        foreach (var line in request.Items)
        {
            var catalogItem = await _menuCatalog.GetItemInfoAsync(line.MenuItemId, ct)
                ?? throw new DomainException($"Позицію меню з Id={line.MenuItemId} не знайдено в каталозі.");

            if (!catalogItem.IsAvailable)
                throw new DomainException($"Позиція «{catalogItem.Name}» наразі недоступна.");

            orderItems.Add(new OrderItem(
                catalogItem.MenuItemId,
                catalogItem.Name,
                new Money(catalogItem.Price),
                line.Quantity));
        }

        var order = new Order(
            Guid.NewGuid(),
            new CustomerName(request.CustomerName),
            orderItems);

        // Розрахунок та застосування найкращої знижки (OCP через IOrderPricingService)
        var discountResult = _pricingService.CalculateBestDiscount(order);
        if (discountResult.DiscountAmount.Amount > 0)
        {
            order.ApplyDiscount(discountResult.DiscountAmount, discountResult.PolicyName);
        }

        await _orderRepository.AddAsync(order, ct);

        // Публікація доменної події про створення замовлення
        await _eventBus.PublishAsync(new OrderCreatedEvent(
            order.Id,
            order.CustomerName.Value,
            order.TotalAmount.Amount,
            order.Items.Count,
            order.CreatedAt), ct);

        return OrderQueryService.MapToResponse(order);
    }

    public async Task<OrderResponse> ChangeStatusAsync(
        Guid id, ChangeStatusRequest request, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Замовлення з Id={id} не знайдено.");

        var previousStatus = order.Status.ToString();

        switch (request.Action.ToLowerInvariant())
        {
            case "prepare":
                order.StartPreparing();
                break;
            case "ready":
                order.MarkAsReady();
                break;
            case "complete":
                order.Complete();
                break;
            case "cancel":
                order.Cancel();
                break;
            default:
                throw new DomainException(
                    $"Невідома дія «{request.Action}». Допустимі: prepare, ready, complete, cancel.");
        }

        await _orderRepository.UpdateAsync(order, ct);

        // Публікація події про зміну життєвого циклу замовлення
        await _eventBus.PublishAsync(new OrderStatusChangedEvent(
            order.Id,
            order.CustomerName.Value,
            previousStatus,
            order.Status.ToString(),
            DateTime.UtcNow), ct);

        return OrderQueryService.MapToResponse(order);
    }
}
