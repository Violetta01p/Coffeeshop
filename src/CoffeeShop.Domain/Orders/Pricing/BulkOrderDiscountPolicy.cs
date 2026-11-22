using CoffeeShop.Domain.Common.ValueObjects;
using CoffeeShop.Domain.Orders.Entities;

namespace CoffeeShop.Domain.Orders.Pricing;

/// <summary>
/// Політика оптової знижки: 10% знижки, якщо у замовленні 3 або більше одиниць товару.
/// </summary>
public class BulkOrderDiscountPolicy : IDiscountPolicy
{
    public string Name => "Оптова знижка 10% (від 3 позицій)";

    public Money CalculateDiscount(Order order)
    {
        var totalQuantity = order.Items.Sum(item => item.Quantity);
        if (totalQuantity >= 3)
        {
            var discountAmount = Math.Round(order.Subtotal.Amount * 0.10m, 2);
            return new Money(discountAmount);
        }

        return Money.Zero;
    }
}
