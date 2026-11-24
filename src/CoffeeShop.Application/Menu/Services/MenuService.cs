using CoffeeShop.Application.Common;
using CoffeeShop.Application.Common.Caching;
using CoffeeShop.Application.Menu.Dtos;
using CoffeeShop.Application.Menu.Mappers;
using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Menu.Entities;
using CoffeeShop.Domain.Menu.Repositories;

namespace CoffeeShop.Application.Menu.Services;

/// <summary>
/// Прикладний сервіс (Use Cases) для контексту Menu.
/// Застосовує патерн Cache-Aside (TTL = 60с) та інвалідацію кешу при зміні даних.
/// Всі операції запису фіксуються атомарно через IUnitOfWork.
/// Мапінг між сутностями та DTO повністю делеговано IMenuMapper.
/// </summary>
public class MenuService : IMenuService
{
    private const string AllMenuCacheKey = "menu:all";
    private readonly IMenuItemRepository _repository;
    private readonly IMenuMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public MenuService(
        IMenuItemRepository repository,
        IMenuMapper mapper,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<MenuItemSummaryDto>> GetAllAsync(CancellationToken ct = default)
    {
        // 1. Спроба отримати з кешу (Cache-Aside)
        var cached = await _cacheService.GetAsync<IReadOnlyList<MenuItemSummaryDto>>(AllMenuCacheKey, ct);
        if (cached is not null)
        {
            return cached;
        }

        // 2. Cache miss — читання з репозиторію
        var items = await _repository.GetAllAsync(ct);
        var dtos = _mapper.MapToSummaryList(items);

        // 3. Збереження в кеш на 60 секунд
        await _cacheService.SetAsync(AllMenuCacheKey, dtos, TimeSpan.FromSeconds(60), ct);

        return dtos;
    }

    public async Task<MenuItemSummaryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"menu:item:{id}";

        // 1. Спроба отримати з кешу (Cache-Aside)
        var cached = await _cacheService.GetAsync<MenuItemSummaryDto>(cacheKey, ct);
        if (cached is not null)
        {
            return cached;
        }

        // 2. Cache miss — читання з репозиторію
        var item = await _repository.GetByIdAsync(id, ct);
        if (item is null)
        {
            return null;
        }

        var dto = _mapper.MapToSummary(item);

        // 3. Збереження в кеш на 60 секунд
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(60), ct);

        return dto;
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

        // Інвалідація списку меню та нової позиції
        await _cacheService.RemoveAsync(AllMenuCacheKey, ct);
        await _cacheService.RemoveAsync($"menu:item:{item.Id}", ct);

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

        // Інвалідація списку меню та зміненої позиції
        await _cacheService.RemoveAsync(AllMenuCacheKey, ct);
        await _cacheService.RemoveAsync($"menu:item:{id}", ct);

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

        // Інвалідація списку меню та зміненої позиції
        await _cacheService.RemoveAsync(AllMenuCacheKey, ct);
        await _cacheService.RemoveAsync($"menu:item:{id}", ct);

        return _mapper.MapToSummary(item);
    }
}
