using CoffeeShop.Domain.Orders.Enums;

namespace CoffeeShop.Application.Orders.Dtos;

// ── Запити ──

public sealed record CreateOrderRequest(
    string CustomerName,
    List<OrderItemRequest> Items);

public sealed record OrderItemRequest(
    Guid MenuItemId,
    int Quantity);

public sealed record ChangeStatusRequest(
    string Action); // "prepare", "ready", "complete", "cancel"

// ── Відповіді ──

public sealed record OrderResponse(
    Guid Id,
    string CustomerName,
    string Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    List<OrderItemResponse> Items,
    decimal Discount = 0,
    string? AppliedDiscount = null);

public sealed record OrderItemResponse(
    Guid Id,
    Guid MenuItemId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal);
