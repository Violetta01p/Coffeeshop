using CoffeeShop.Application.Menu.Dtos;

namespace CoffeeShop.Application.Menu.Services;

/// <summary>
/// Інтерфейс сервісу меню (DIP).
/// </summary>
public interface IMenuService
{
    Task<IReadOnlyList<MenuItemResponse>> GetAllAsync(CancellationToken ct = default);
    Task<MenuItemResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MenuItemResponse> CreateAsync(CreateMenuItemRequest request, CancellationToken ct = default);
    Task<MenuItemResponse> UpdateAvailabilityAsync(Guid id, UpdateAvailabilityRequest request, CancellationToken ct = default);
    Task<MenuItemResponse> UpdatePriceAsync(Guid id, UpdatePriceRequest request, CancellationToken ct = default);
}
