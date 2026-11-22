using CoffeeShop.Application.Common;
using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Application.Menu.Mappers;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Repositories;

namespace CoffeeShop.Application.Menu.Services;

/// <summary>
/// Прикладний сервіс (Use Cases) для контексту Menu.
/// Всі операції запису фіксуються атомарно через IUnitOfWork.
/// Мапінг між сутностями та DTO повністю делеговано IMenuMapper.
/// </summary>
public class MenuService : IMenuService
{
    private readonly IMenuItemRepository _repository;
    private readonly IMenuMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MenuService(
        IMenuItemRepository repository,
        IMenuMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<MenuItemSummaryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return _mapper.MapToSummaryList(items);
    }

    public async Task<MenuItemSummaryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct);
        return item is null ? null : _mapper.MapToSummary(item);
    }

    public async Task<MenuItemSummaryDto> CreateAsync(CreateMenuItemRequestDto request, CancellationToken ct = default)
    {
        var category = _mapper.MapToCategory(request.Category);

        var item = new MenuItem(
            Guid.NewGuid(),
            request.Name,
            category,
            new Money(request.Price));

        await _repository.AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.MapToSummary(item);
    }

    public async Task<MenuItemSummaryDto> UpdateAvailabilityAsync(
        Guid id, UpdateMenuItemAvailabilityRequestDto request, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Позицію меню з Id={id} не знайдено.");

        if (request.IsAvailable)
            item.MarkAsAvailable();
        else
            item.MarkAsUnavailable();

        await _repository.UpdateAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.MapToSummary(item);
    }

    public async Task<MenuItemSummaryDto> UpdatePriceAsync(
        Guid id, UpdateMenuItemPriceRequestDto request, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Позицію меню з Id={id} не знайдено.");

        item.UpdatePrice(new Money(request.Price));
        await _repository.UpdateAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.MapToSummary(item);
    }
}
