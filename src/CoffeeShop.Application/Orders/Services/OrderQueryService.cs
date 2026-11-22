using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Mappers;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Прикладний сервіс запитів (Queries) — відповідає лише за читання замовлень (SRP / ISP).
/// Мапінг делеговано інжектованому IOrderMapper (IoC / DIP).
/// </summary>
public class OrderQueryService : IOrderQueryService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderMapper _mapper;

    public OrderQueryService(IOrderRepository orderRepository, IOrderMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        return _mapper.ToResponseList(orders);
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.ToResponse(order);
    }
}
