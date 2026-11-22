using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Application.Orders.Mappers;

/// <summary>
/// Stateless-мапер замовлень (Transient).
/// Реалізує мапінг доменних сутностей у спеціалізовані вхідні/вихідні DTO (Summary, Details).
/// </summary>
public class OrderMapper : IOrderMapper
{
    public OrderSummaryDto MapToSummary(Order order) =>
        new(order.Id,
            order.CustomerName.Value,
            order.Status.ToString(),
            order.Items.Count,
            order.TotalAmount.Amount,
            DateTime.SpecifyKind(order.CreatedAt, DateTimeKind.Utc));

    public IReadOnlyList<OrderSummaryDto> MapToSummaryList(IEnumerable<Order> orders) =>
        orders.Select(MapToSummary).ToList();

    public OrderDetailsDto MapToDetails(Order order) =>
        new(order.Id,
            order.CustomerName.Value,
            order.Status.ToString(),
            DateTime.SpecifyKind(order.CreatedAt, DateTimeKind.Utc),
            order.TotalAmount.Amount,
            MapItemsToDtoList(order.Items),
            order.Discount.Amount,
            order.AppliedDiscountName);

    public OrderItemDto MapItemToDto(OrderItem item) =>
        new(item.Id,
            item.MenuItemId,
            item.ProductName,
            item.UnitPrice.Amount,
            item.Quantity,
            item.Subtotal.Amount);

    public IReadOnlyList<OrderItemDto> MapItemsToDtoList(IEnumerable<OrderItem> items) =>
        items.Select(MapItemToDto).ToList();
}
