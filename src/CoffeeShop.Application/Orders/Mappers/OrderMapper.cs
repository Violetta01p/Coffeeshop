using CoffeeShop.Application.Orders.Dtos;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Application.Orders.Mappers;

/// <summary>
/// Stateless-мапер замовлень (Transient).
/// </summary>
public class OrderMapper : IOrderMapper
{
    public OrderResponse ToResponse(Order order) =>
        new(order.Id,
            order.CustomerName.Value,
            order.Status.ToString(),
            order.CreatedAt,
            order.TotalAmount.Amount,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.MenuItemId,
                i.ProductName,
                i.UnitPrice.Amount,
                i.Quantity,
                i.Subtotal.Amount)).ToList(),
            order.Discount.Amount,
            order.AppliedDiscountName);

    public IReadOnlyList<OrderResponse> ToResponseList(IEnumerable<Order> orders) =>
        orders.Select(ToResponse).ToList();
}
