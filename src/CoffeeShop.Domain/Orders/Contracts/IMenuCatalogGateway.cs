namespace CoffeeShop.Domain.Orders.Contracts;

/// <summary>
/// Контракт взаємодії контексту Orders із контекстом Menu.
/// Дозволяє перевіряти доступність позиції та отримувати актуальну ціну
/// без прямої залежності від внутрішньої реалізації Menu.
/// </summary>
public interface IMenuCatalogGateway
{
    /// <summary>
    /// Отримати інформацію про позицію каталогу за ідентифікатором.
    /// Повертає null, якщо позицію не знайдено.
    /// </summary>
    Task<CatalogItemInfo?> GetItemInfoAsync(Guid menuItemId, CancellationToken ct = default);
}

/// <summary>
/// DTO-контракт (Data Transfer Object), який передає мінімум даних
/// із контексту Menu до контексту Orders.
/// </summary>
public sealed record CatalogItemInfo(
    Guid MenuItemId,
    string Name,
    decimal Price,
    bool IsAvailable);
