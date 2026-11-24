using CoffeeShop.Application.Common.Caching;
using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Mappers;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Прикладний сервіс запитів (Queries) — відповідає лише за читання замовлень (SRP / ISP).
/// Реалізує патерн Cache-Aside із збереженням деталей замовлень у розподіленому кеші (TTL = 60с).
/// Мапінг сутностей у вихідні DTO повністю делеговано інжектованому IOrderMapper (IoC / DIP).
/// </summary>
public class OrderQueryService : IOrderQueryService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderMapper _mapper;
    private readonly ICacheService _cacheService;

    public OrderQueryService(
        IOrderRepository orderRepository,
        IOrderMapper mapper,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<OrderSummaryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        return _mapper.MapToSummaryList(orders);
    }

    public async Task<OrderDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"order:{id}";

        // 1. Спроба отримати з кешу (Cache-Aside)
        var cachedOrder = await _cacheService.GetAsync<OrderDetailsDto>(cacheKey, ct);
        if (cachedOrder is not null)
        {
            return cachedOrder;
        }

        // 2. Cache miss — читання з репозиторію
        var order = await _orderRepository.GetByIdAsync(id, ct);
        if (order is null)
        {
            return null;
        }

        var dto = _mapper.MapToDetails(order);

        // 3. Збереження в кеш із TTL = 60 секунд
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(60), ct);

        return dto;
    }
}
