using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Application.Orders.Mappers;
using CoffeeShop.Domain.Orders.Repositories;

namespace CoffeeShop.Application.Orders.Services;

/// <summary>
/// Прикладний сервіс запитів (Queries) — відповідає лише за читання замовлень (SRP / ISP).
/// Мапінг сутностей у вихідні DTO повністю делеговано інжектованому IOrderMapper (IoC / DIP).
/// Жодного ручного new ...Dto(...) всередині сервісу не виконується.
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

    public async Task<IReadOnlyList<OrderSummaryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        return _mapper.MapToSummaryList(orders);
    }

    public async Task<OrderDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        return order is null ? null : _mapper.MapToDetails(order);
    }
}
