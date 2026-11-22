using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Application.Orders.Mappers;

/// <summary>
/// Інтерфейс мапера між сутностями контексту Orders та DTO (Transient).
/// Забезпечує ізоляцію доменних сутностей та централізоване перетворення моделей.
/// </summary>
public interface IOrderMapper
{
    OrderSummaryDto MapToSummary(Order order);
    IReadOnlyList<OrderSummaryDto> MapToSummaryList(IEnumerable<Order> orders);
    OrderDetailsDto MapToDetails(Order order);
    OrderItemDto MapItemToDto(OrderItem item);
    IReadOnlyList<OrderItemDto> MapItemsToDtoList(IEnumerable<OrderItem> items);
}
