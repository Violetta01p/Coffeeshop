namespace CoffeeShop.Application.Menu.Dtos;

// ── Вхідні DTO (Requests) ──

/// <summary>
/// Вхідний DTO для створення нової позиції меню (POST /api/Menu).
/// Category приймається як string ("HotDrink", "ColdDrink", "Dessert", "Snack"),
/// щоб унеможливити витік доменного enum у шар WebApi.
/// </summary>
public sealed record CreateMenuItemRequestDto(
    string Name,
    string Category,
    decimal Price);

/// <summary>
/// Вхідний DTO для зміни наявності позиції меню (PATCH /api/Menu/{id}/availability).
/// </summary>
public sealed record UpdateMenuItemAvailabilityRequestDto(
    bool IsAvailable);

/// <summary>
/// Вхідний DTO для зміни ціни позиції меню (PATCH /api/Menu/{id}/price).
/// </summary>
public sealed record UpdateMenuItemPriceRequestDto(
    decimal Price);

// ── Вихідні DTO (Responses) ──

/// <summary>
/// Спеціалізований вихідний DTO для представлення позиції меню (GET /api/Menu, GET /api/Menu/{id}).
/// </summary>
public sealed record MenuItemSummaryDto(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    bool IsAvailable);
