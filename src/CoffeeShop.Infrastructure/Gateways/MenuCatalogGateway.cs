using CoffeeShop.Domain.Menu.Repositories;
using CoffeeShop.Domain.Orders.Contracts;

namespace CoffeeShop.Infrastructure.Gateways;

/// <summary>
/// Реалізація контракту IMenuCatalogGateway.
/// Мост між контекстами Orders → Menu через репозиторій Menu.
/// </summary>
public class MenuCatalogGateway : IMenuCatalogGateway
{
    private readonly IMenuItemRepository _menuItemRepository;

    public MenuCatalogGateway(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<CatalogItemInfo?> GetItemInfoAsync(Guid menuItemId, CancellationToken ct = default)
    {
        var item = await _menuItemRepository.GetByIdAsync(menuItemId, ct);
        if (item is null)
            return null;

        return new CatalogItemInfo(
            item.Id,
            item.Name,
            item.Price.Amount,
            item.IsAvailable);
    }
}
