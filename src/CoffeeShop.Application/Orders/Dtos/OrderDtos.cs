namespace CoffeeShop.Application.Orders.Dtos;

// ── Вхідні DTO (Requests) ──

/// <summary>
/// Вхідний DTO для окремої позиції у запиті на створення замовлення.
/// </summary>
public sealed record OrderItemRequestDto(
    Guid MenuItemId,
    int Quantity);

/// <summary>
/// Вхідний DTO для створення нового замовлення (POST /api/Orders).
/// </summary>
public sealed record CreateOrderRequestDto(
    string CustomerName,
    List<OrderItemRequestDto> Items);

/// <summary>
/// Вхідний DTO для зміни статусу замовлення (PATCH /api/Orders/{id}/status).
/// Підтримує гнучку передачу дії (prepare, ready, complete, cancel) або назви статусу.
/// </summary>
public sealed record UpdateOrderStatusRequestDto(
    string? Status = null,
    string? Action = null)
{
    public string TargetAction => !string.IsNullOrWhiteSpace(Status) ? Status : (Action ?? string.Empty);
}

// ── Вихідні DTO (Responses) ──

/// <summary>
/// Деталізований вихідний DTO позиції замовлення.
/// </summary>
public sealed record OrderItemDto(
    Guid Id,
    Guid MenuItemId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal);

/// <summary>
/// Компактний вихідний DTO для відображення у списку замовлень (GET /api/Orders).
/// Оптимізований за обсягом: не завантажує масив позицій.
/// </summary>
public sealed record OrderSummaryDto(
    Guid Id,
    string CustomerName,
    string Status,
    int ItemsCount,
    decimal TotalAmount,
    DateTime CreatedAtUtc);

/// <summary>
/// Повний вихідний DTO для детального перегляду замовлення (GET /api/Orders/{id})
/// та повернення результатів створення/зміни статусу.
/// </summary>
public sealed record OrderDetailsDto(
    Guid Id,
    string CustomerName,
    string Status,
    DateTime CreatedAtUtc,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items,
    decimal Discount = 0,
    string? AppliedDiscount = null);
