using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Application.Orders.Mappers;

/// <summary>
/// Інтерфейс мапера сутностей Orders у DTO (Transient).
/// </summary>
public interface IOrderMapper
{
    OrderResponse ToResponse(Order order);
    IReadOnlyList<OrderResponse> ToResponseList(IEnumerable<Order> orders);
}
