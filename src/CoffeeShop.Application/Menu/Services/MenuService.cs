using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Repositories;

namespace CoffeeShop.Application.Menu.Services;

/// <summary>
/// Прикладний сервіс (Use Cases) для контексту Menu.
/// </summary>
public class MenuService : IMenuService
{
    private readonly IMenuItemRepository _repository;

    public MenuService(IMenuItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MenuItemResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<MenuItemResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct);
        return item is null ? null : MapToResponse(item);
    }

    public async Task<MenuItemResponse> CreateAsync(CreateMenuItemRequest request, CancellationToken ct = default)
    {
        var item = new MenuItem(
            Guid.NewGuid(),
            request.Name,
            request.Category,
            new Money(request.Price));

        await _repository.AddAsync(item, ct);
        return MapToResponse(item);
    }

    public async Task<MenuItemResponse> UpdateAvailabilityAsync(
        Guid id, UpdateAvailabilityRequest request, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Позицію меню з Id={id} не знайдено.");

        if (request.IsAvailable)
            item.MarkAsAvailable();
        else
            item.MarkAsUnavailable();

        await _repository.UpdateAsync(item, ct);
        return MapToResponse(item);
    }

    public async Task<MenuItemResponse> UpdatePriceAsync(
        Guid id, UpdatePriceRequest request, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Позицію меню з Id={id} не знайдено.");

        item.UpdatePrice(new Money(request.Price));
        await _repository.UpdateAsync(item, ct);
        return MapToResponse(item);
    }

    // ── Mapping ──

    private static MenuItemResponse MapToResponse(MenuItem item) =>
        new(item.Id,
            item.Name,
            item.Category.ToString(),
            item.Price.Amount,
            item.IsAvailable);
}
