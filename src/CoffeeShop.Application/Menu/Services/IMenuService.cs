using CoffeeShop.Application.Menu.Dtos;

namespace CoffeeShop.Application.Menu.Services;

/// <summary>
/// Інтерфейс сервісу меню (DIP).
/// Працює виключно зі спеціалізованими DTO.
/// </summary>
public interface IMenuService
{
    Task<IReadOnlyList<MenuItemSummaryDto>> GetAllAsync(CancellationToken ct = default);
    Task<MenuItemSummaryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MenuItemSummaryDto> CreateAsync(CreateMenuItemRequestDto request, CancellationToken ct = default);
    Task<MenuItemSummaryDto> UpdateAvailabilityAsync(Guid id, UpdateMenuItemAvailabilityRequestDto request, CancellationToken ct = default);
    Task<MenuItemSummaryDto> UpdatePriceAsync(Guid id, UpdateMenuItemPriceRequestDto request, CancellationToken ct = default);
}
