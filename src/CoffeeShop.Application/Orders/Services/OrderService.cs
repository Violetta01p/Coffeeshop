using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Domain.Common.Exceptions;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Contracts;
using CoffeeShop.Domain.Orders.Entities;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Прикладний сервіс (Use Cases) для контексту Orders.
/// </summary>
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuCatalogGateway _menuCatalog;

    public OrderService(IOrderRepository orderRepository, IMenuCatalogGateway menuCatalog)
    {
        _orderRepository = orderRepository;
        _menuCatalog = menuCatalog;
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

        await _orderRepository.AddAsync(order, ct);
        return MapToResponse(order);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        return order is null ? null : MapToResponse(order);
    }

    public async Task<OrderResponse> ChangeStatusAsync(
        Guid id, ChangeStatusRequest request, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Замовлення з Id={id} не знайдено.");

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
        return MapToResponse(order);
    }

    // ── Mapping ──

    private static OrderResponse MapToResponse(Order order) =>
        new(order.Id,
            order.CustomerName.Value,
            order.Status.ToString(),
            order.CreatedAt,
            order.TotalAmount.Amount,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.MenuItemId,
                i.ProductName,
                i.UnitPrice.Amount,
                i.Quantity,
                i.Subtotal.Amount)).ToList());
}
