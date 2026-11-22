using CoffeeShop.Domain.Menu.Enums;

namespace CoffeeShop.Application.Menu.Dtos;

// ── Запити ──

public sealed record CreateMenuItemRequest(
    string Name,
    MenuCategory Category,
    decimal Price);

public sealed record UpdateAvailabilityRequest(
    bool IsAvailable);

public sealed record UpdatePriceRequest(
    decimal Price);

// ── Відповіді ──

public sealed record MenuItemResponse(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    bool IsAvailable);
